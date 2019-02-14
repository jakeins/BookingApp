-- ==========================================================
-- Author:		Ratushnyiy Oleksiy
-- Create date: 06.02.2019
-- Description:	Modify exist booking, @BookingID 
--				and @EditUserID are mandatory other optional,
--				@StartTime and @EndTime must be datetime 
--				future( > Current time stamp), set modify 
--				paramete, other set Null
-- ==========================================================
CREATE PROCEDURE [dbo].[Booking.Edit]
	@BookingID int,
	@StartTime datetime,
	@EndTime datetime,
	@EditUseID nvarchar(450),
	@Note nvarchar(max)
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

	-- verify that user exist
	if not exists (Select AspNetUsers.Id From AspNetUsers Where AspNetUsers.Id = @EditUseID)
		Throw 50001, 'Invalid user id', 2;

	-- declare variable for transaction name
	Declare @TransactioName nvarchar(max);
	-- set transaction name
	Set @TransactioName = 'Booking ' + CAST(@BookingID AS nvarchar(max))  

	-- verify is time not setted
	if (@StartTime Is Null) And (@EndTime Is Null)
	-- if true verify is @Note setted, if not setted end execution
	Begin
		If @Note Is Null
			Return;
		-- else begin transaction for set new @Note of booking
		Begin Transaction @TransactioName;
		Begin
			-- verify is booking exist
			if Not Exists (Select Bookings.Id From Bookings Where Bookings.Id = @BookingID)
				Throw 50001, 'Invalid BookingID',  12;
			--verify that booking not terminated
			If (Select Bookings.TerminationTime From Bookings Where Bookings.Id = @BookingID) Is Not Null
				Throw 50001, 'Can not edit termunated booking',  13;
			-- set new Note, change UpdateTime to current time stamp and UserID to @EditUserID
			Update Bookings
			Set Bookings.Note = @Note, Bookings.UpdatedUserId = @EditUseID, Bookings.UpdatedTime = @BookingTimeStamp
			Where Bookings.Id = @BookingID;
		End
		-- commit transaction and execution of procedure
		Commit Transaction @TransactioName;
		Return;
	End

	-- verify is @StartTime not null and set in future
	if @StartTime Is Not Null 
		if @StartTime < @BookingTimeStamp
			Throw 50001, 'Cannot modify StartTime when its in past', 1;
	-- verify is @EndTime not null and set in future
	if @EndTime Is Not Null
		If @EndTime < @BookingTimeStamp
			Throw 50001, 'Cannot modify EndTime when its in past', 1;
	-- verify is @StartTime lower than @EndTime
	if (@StartTime Is Not Null) And (@EndTime Is Not Null)
		if not (@StartTime < @EndTime)
			Throw 50001, 'StartTime must be lower than EndTime', 3;

	-- begin transaction to change time window of booking
	Begin Transaction @TransactioName
	Begin
		-- verify that booking not terminated
		If (Select Bookings.TerminationTime From Bookings Where Bookings.Id = @BookingID) Is Not Null
			Throw 50001, 'Can not edit termіnated booking',  13;
		-- verify that booking not ended
		If (Select Bookings.EndTime From Bookings Where Bookings.Id = @BookingID) <= @BookingTimeStamp
			Throw 50001, 'Can not edit ended booking',  14;
		-- declare table for storing current booking data
		Declare @BookingData Table(
			ID int, 
			StartTime datetime, 
			EndTime datetime, 
			ResourceId int,  
			UserID nvarchar(450), 
			Note nvarchar(max)
			);
		-- get current booking data
		Insert Into 
			@BookingData(
				ID, 
				StartTime, 
				EndTime, 
				ResourceId,
				UserID, 
				Note
			) 
		Select 
			Bookings.BookingId, 
			Bookings.StartTime, 
			Bookings.EndTime, 
			Bookings.ResourceId,  
			Bookings.CreatedUserId, 
			Bookings.Note
		From Bookings 
		Where Bookings.Id = @BookingID;
		-- if data not exit than invalid @BookingID passed, throw exception
		IF Not Exists (SELECT 1 FROM @BookingData)
			Throw 50001, 'Invalid BookingID',  12;
	
		-- declare variable to store new time
		Declare @NewStartTime datetime;
		Declare @NewEndTime datetime;

		-- verify that booking not started
		if (Select Top 1 StartTime From @BookingData) <= @BookingTimeStamp
			Throw 50001, 'Can not change starttime of alredy started booking',  15;

		-- if @StartTime seted than use it for @NewStartTime else use current EndTime for this Booking
		If @StartTime Is Not Null
			Set @NewStartTime = @StartTime;	
		Else
			Set @NewStartTime = (Select Top 1 StartTime From @BookingData);

		-- if @EndTime seted than use it for @NewEndTime else use current EndTime for this Booking
		If @EndTime Is Not Null
			Set @NewEndTime = @EndTime;
		Else
			Set @NewEndTime = (Select Top 1 EndTime From @BookingData);

		-- get resource book rule if resource and rule is active
		Declare @Rule Table(MaxTime int, MinTime int, StepTime int, ServiceTime int, ReuseTimeout int, PreOrderTimeLimit int);
		Insert Into @Rule
			Select Rules.MaxTime, Rules.MinTime, Rules.StepTime, Rules.ServiceTime, Rules.ReuseTimeout, Rules.PreOrderTimeLimit
				From Rules Where Rules.IsActive = 1 AND Rules.Id = 
					(Select Resources.RuleID From Resources 
						Where Resources.IsActive = 1 AND Resources.Id = (Select Top 1 ID From @BookingData));

		-- verify that rule is active
		If Not Exists (Select 1 From @Rule)
			Throw 50001, 'Rule or resources is disabled', 6;

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
		Set @Duration = DATEDIFF(minute, @NewStartTime, @NewEndTime);
		-- verify that duration more then minimal time and less then max time valid for booking this resource
		if @Duration < @MinValidTime
			Throw 50000, 'Booking duration less than min valid for this resource', 7;
		if @Duration > @MaxValidTime
			Throw 50000, 'Booking duration more than max valid for this resource', 8;

		-- verify that duration is multiple by step of the booking this resource
		if @Duration % @ValidStepTime != 0
			Throw 50000, 'The duration of the reservation must be a multiple step of the booking for this resource', 9;

		-- verify that resource not being booked too early
		if DATEADD(minute, @PreOrderTimeLimit, @BookingTimeStamp) < @NewStartTime
			Throw 50000, 'Booking time is too early', 10;

		-- declare counter
		Declare @CountBooksInSameTime int;
		-- calculate count bookings in same time using dbo.Booking.IsRangeAvailable function
		-- notes: weigh that the creator is a customer	
		Set @CountBooksInSameTime =(
			Select Count(Bookings.Id)
						From Bookings 
						Where Bookings.ResourceId = (Select Top 1 ID From @BookingData) 
						AND
						(Select [dbo].[Booking.IsRangeAvailable](
							@NewStartTime, 
							@NewEndTime, 
							(Select Top 1 UserID From @BookingData),
							Bookings.StartTime, 
							Bookings.EndTime, 
							Bookings.TerminationTime,
							@ServiceTime,
							@ReuseTimeoutPerUser,
							Bookings.CreatedUserId) As Result 
						)= 1
				);
		-- verify is no booking in same time
		if (@CountBooksInSameTime > 0)
			Throw 50001, 'Time range alredy booked', 11;
		-- update booking time and 

		UPDATE Bookings 
		Set Bookings.StartTime = @NewStartTime,
			Bookings.EndTime = @NewEndTime,
			Bookings.TerminationTime = null,
			Bookings.UpdatedUserId = @EditUseID,
			Bookings.UpdatedTime = @BookingTimeStamp,
			Bookings.Note = ISNULL(@Note, (SELECT Top 1 Note From @BookingData))
		Where
			Bookings.Id = (Select Top 1 ID From @BookingData) 
	End;
	Commit Transaction @TransactioName;
END
