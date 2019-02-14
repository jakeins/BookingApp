-- =============================================
-- Author:		Ratushnyiy Oleksiy
-- Create date: 03-02-2019
-- Description:	Verify args and create booking if time window free
-- =============================================
CREATE PROCEDURE [dbo].[Booking.Create]
	@ResourceID int,
	@StartTime DateTime,
	@EndTime DateTime,
	@UserID nvarchar(450),
	@Note nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- SET XACT_ABORT ON rollback transaction when exception throwed
	SET XACT_ABORT, NOCOUNT ON;

	-- declare variable for tamestamping
	Declare @BookingTimeStamp datetime;
	-- get current date and time
	Set @BookingTimeStamp = CURRENT_TIMESTAMP;

	-- verify is starttime is in future because cant book resource in past
	if @BookingTimeStamp > @StartTime 
		Throw 50001, 'Can not book when StartTime in past', 1;

	-- verify that user exist
	if not exists (Select AspNetUsers.Id From AspNetUsers Where AspNetUsers.Id = @UserID)
		Throw 50001, 'Invalid user id', 2;
	-- verify that start time early then end time
	if not (@StartTime < @EndTime)
		Throw 50001, 'StartTime must be lower than EndTime', 3;
	-- verify that resource exist
	if not exists(Select Resources.RuleID From Resources 
					Where Resources.Id = @ResourceID)
		Throw 50001, 'Invalid resource id passed', 4;
	-- verify that resource active
	if not exists (Select Resources.RuleID From Resources 
					Where Resources.Id = @ResourceID AND Resources.IsActive = 1)
		Throw 50002, 'Resorce is disable and can not book', 5;

	-- get resource book rule
	Declare @Rule Table(MaxTime int, MinTime int, StepTime int, ServiceTime int, ReuseTimeout int, PreOrderTimeLimit int);
	Insert Into @Rule
		Select Rules.MaxTime, Rules.MinTime, Rules.StepTime, Rules.ServiceTime, Rules.ReuseTimeout, Rules.PreOrderTimeLimit
			From Rules Where Rules.IsActive = 1 AND Rules.Id = 
				(Select Resources.RuleID From Resources 
					Where Resources.Id = @ResourceID);

	-- verify that rule is active
	If Not Exists (Select 1 From @Rule)
		Throw 50001, 'Rule is disabled for this resource', 6;

	-- declare variables for rule options
	Declare @MaxValidTime int;
	Declare @MinValidTime int;
	Declare @ValidStepTime int;
	Declare @ServiceTime int;
	Declare @ReuseTimeoutPerUser int;
	Declare @PreOrderTimeLimit int;
	-- extract rule options
	Set @MaxValidTime = (Select Top 1 MaxTime From @Rule);
	Set @MinValidTime = (Select Top 1 MinTime From @Rule);
	Set @ValidStepTime = (Select Top 1 StepTime From @Rule);
	Set @ServiceTime = (Select Top 1 ServiceTime From @Rule);
	Set @ReuseTimeoutPerUser = (Select Top 1 ReuseTimeout From @Rule);
	Set @PreOrderTimeLimit = (Select Top 1 PreOrderTimeLimit From @Rule);

	-- declare and computate duration of requested booking in minutes
	Declare @Duration int;
	Set @Duration = DATEDIFF(minute, @StartTime, @EndTime);
	-- verify that duration more then minimal time and less then max time valid for booking this resource
	if @Duration < @MinValidTime
		Throw 50000, 'Booking duration less than min valid for this resource', 7;
	if @Duration > @MaxValidTime
		Throw 50000, 'Booking duration more than max valid for this resource', 8;

	-- verify that duration is multiple by step of the booking this resource
	if @Duration % @ValidStepTime != 0
		Throw 50000, 'The duration of the reservation must be a multiple step of the booking for this resource', 9;

	-- verify that resource not being booked too early
	if DATEADD(minute, @PreOrderTimeLimit, @BookingTimeStamp) < @StartTime
		Throw 50000, 'Booking time is too early', 10;

	-- declare variable for storing newly created bookin id
	Declare @BookingID int;

	-- create transaction for booking
	Begin Transaction Booking
	Begin
		-- declare counter
		DECLARE @CountBooksInSameTime int;
		-- calculate count bookings in same time using dbo.Booking.IsRangeAvailable function
		-- notes: weigh that the creator is a customer	
		SET @CountBooksInSameTime =(
			Select Count(Bookings.Id)
						From Bookings 
						Where Bookings.ResourceId = @ResourceID 
						AND
						(SELECT [dbo].[Booking.IsRangeAvailable](
							@StartTime, 
							@EndTime, 
							@UserID,
							Bookings.StartTime, 
							Bookings.EndTime, 
							Bookings.TerminationTime,
							@ServiceTime,
							@ReuseTimeoutPerUser,
							Bookings.CreatedUserId) AS Result 
						)= 1
				);
		-- verify is no booking in same time
		if (@CountBooksInSameTime > 0)
			Throw 50001, 'Time range alredy booked', 11;
		-- declare temp table variable for store insert result
		Declare @InsertResult table(Id int);
		-- insert booking to bookings table and get his id
		Insert Into Bookings(ResourceID, StartTime, EndTime, CreatedTime, UpdatedTime, CreatedUserID, UpdatedUserID, Note)
		Output INSERTED.Id Into @InsertResult
		Values(@ResourceID, @StartTime, @EndTime, @BookingTimeStamp, @BookingTimeStamp, @UserID, @UserID, @Note);
		-- extract id from temp result table
		Set @BookingID = (Select Top 1 Id From @InsertResult);
		-- commit changes
	End;
	Commit Transaction Booking;
	-- return id of newly created booking
	Return Select Top 1 * From Bookings Where Bookings.Id = @BookingID
END