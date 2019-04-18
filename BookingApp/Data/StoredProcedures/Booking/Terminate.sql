-- ==========================================================
-- Author:		Ratushnyiy Oleksiy
-- Create date: 06.02.2019
-- Description: Terminate exist booking
-- ==========================================================
CREATE PROCEDURE [dbo].[Booking.Terminate]
	@BookingID int,
	@UserID nvarchar(450)
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

	---- verify that user exist
	if not exists (Select AspNetUsers.Id From AspNetUsers Where AspNetUsers.Id = @UserID)
		Throw 50001, 'Invalid user id', 2;

	-- declare variable for transaction name
	Declare @TransactioName nvarchar(max);
	-- set transaction name
	Set @TransactioName = 'Booking ' + CAST(@BookingID AS nvarchar(max))  
	-- start transaction
	Begin Transaction @TransactioName;
	Begin
		-- verify is booking exist
		if Not Exists (Select Bookings.Id From Bookings Where Bookings.Id = @BookingID)
			Throw 50001, 'Invalid BookingID',  12;

		--verify that booking not terminated
		If (Select Bookings.TerminationTime From Bookings Where Bookings.Id = @BookingID) Is Not Null
			Throw 50001, 'Alredy terminated',  13;

		-- verify that booking not ended
		If (Select Bookings.EndTime From Bookings Where Bookings.Id = @BookingID) <= @BookingTimeStamp
			Throw 50001, 'Can not terminate ended booking',  13;

		-- set terminate time for booking
		UPDATE Bookings
		Set Bookings.TerminationTime = @BookingTimeStamp,
			Bookings.UpdatedUserId = @UserID
		Where Bookings.Id = @BookingID;
	End;
	-- commit transaction
	Commit Transaction @TransactioName;
End;