-- =============================================
-- Author:		Ratushnyiy Oleksiy
-- Create date: 03-02-2019
-- Description:	Verify args and create booking if time window free
-- =============================================
CREATE PROCEDURE [dbo].[Booking.Create]
	-- Add the parameters for the stored procedure here
	@ResourceID int,
	@StartTime DateTime,
	@EndTime DateTime,
	@UserID nvarchar(450),
	@Note nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Check is user exist
	if not exists (Select AspNetUsers.Id From AspNetUsers Where AspNetUsers.Id = @UserID)
		THROW 50001, 'Invalid user id', 1;
	-- Check is start time early then end time
	if not (@StartTime < @EndTime)
		Throw 50001, 'StartTime must be lower than EndTime', 2;
	-- Check is resource exist
	if not exists(Select Resources.RuleID From Resources 
					Where Resources.ResourceId = @ResourceID)
		Throw 50001, 'Invalid resource id passed', 3;
	-- Get resource book rule
	Declare @Rule Table(MaxTime int, MinTime int, StepTime int, ServiceTime int);
	INSERT INTO @Rule
		SELECT Rules.MaxTime, Rules.MinTime, Rules.StepTime, Rules.ServiceTime
			From Rules Where Rules.RuleId = 
				(Select Resources.RuleID From Resources 
					Where Resources.ResourceId = @ResourceID);
	--Declare variables for rule options
	Declare @MaxValidTime int;
	Declare @MinValidTime int;
	Declare @ValidStepTime int;
	Declare @ServiceTime int;
	-- Extract rule options
	Set @MaxValidTime = (Select Top 1 MaxTime From @Rule);
	Set @MinValidTime = (Select Top 1 MinTime From @Rule);
	Set @ValidStepTime = (Select Top 1 StepTime From @Rule);
	Set @ServiceTime = (Select Top 1 ServiceTime From @Rule);
	--Declare and computate duration of requested booking in minutes
	Declare @Duration int;
	Set @Duration = DATEDIFF(minute, @StartTime, @EndTime);
	--Verify is duration more then minimal time and less then max time valid for booking this resource
	if @Duration < @MinValidTime
		Throw 50000, 'Booking duration less than min valid for this resource', 4;
	if @Duration > @MaxValidTime
		Throw 50000, 'Booking duration more than max valid for this resource', 5;
	--Verify is duration is multiple by step of the booking this resource
	if @Duration % @ValidStepTime != 0
		Throw 50000, 'The duration of the reservation must be a multiple step of the booking for this resource', 6;

	--Create transaction for booking
	Begin Transaction Booking
	--Declare bookings data
	DECLARE @BookingStartTime datetime
	DECLARE @BookingEndTime datetime
	DECLARE @BookingTerminationTime datetime
	--Declare counter
	DECLARE @CountBooksInSameTime int;
	--Declare iterator for iterating in booking with specific resource id
	DECLARE Iterator Cursor FAST_FORWARD
		For Select Bookings.StartTime, Bookings.EndTime, Bookings.TerminationTime
				From Bookings 
				Where Bookings.ResourceId = @ResourceID
	--Open iterator
	OPEN Iterator
	--Declare variable for storing result of checking time range
	DECLARE @result int
	--Set initial variable for counter
	SET @CountBooksInSameTime = 0;
	--Get first row
	FETCH NEXT FROM Iterator INTO @BookingStartTime, @BookingEndTime, @BookingTerminationTime
	--While can fetch next row
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--Call checking for time range if range not available result will be 1 else 0
		Exec @result = IsRangeAvailable @StartTime, @EndTime, @BookingStartTime, @BookingEndTime, @BookingTerminationTime, @ServiceTime
		--Add result to counter
		SET @CountBooksInSameTime = @CountBooksInSameTime + @result
		--Try get next row
		FETCH NEXT FROM Iterator INTO @BookingStartTime, @BookingEndTime, @BookingTerminationTime
	END
	--Close and destroy iterator
	CLOSE Iterator
	DEALLOCATE Iterator
	--Verify is no booking in same time
	if (@CountBooksInSameTime > 0)
		Begin;
		ROLLBACK TRANSACTION Booking;
		THROW 50001, 'Time range alredy booked', 7;
		End;
	--Declare variable for tamestamping
	Declare @BookingTimeStamp datetime;
	--Get current date and time
	Set @BookingTimeStamp = CURRENT_TIMESTAMP;
	--Insert booking to bookings table
	Insert Into Bookings(ResourceID, StartTime, EndTime, CreatedTime, UpdatedTime, CreatedUserID, UpdatedUserID, Note)
	Values(@ResourceID, @StartTime, @EndTime, @BookingTimeStamp, @BookingTimeStamp, @UserID, @UserID, @Note);
	--commit changes
	Commit Transaction Booking;
END