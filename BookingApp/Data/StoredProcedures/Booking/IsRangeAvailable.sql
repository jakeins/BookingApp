-- ============================================================
-- Author:		Ratushnyiy Oleksiy
-- Create date: 05.02.2019
-- Description:	Check is booking range available,
--              return 1 if not applicable and 0 if applicable
-- ============================================================
CREATE FUNCTION [dbo].[Booking.IsRangeAvailable]
(
	@RequestStartTime datetime,
	@RequestEndTime datetime,
	@RequestUserID nvarchar(450),
	@BookingStartTime datetime,
	@BookingEndTime datetime,
	@BookingTerminationTime datetime,
	@BookingServiceTime int,
	@BookingPerUserTimeout int,
	@BookingUserID nvarchar(450)
)
RETURNS BIT
AS
BEGIN
	Declare @RealEndTime datetime 
	--if booking termination time equal 0 than booking not canceled and calculate realtime using endtime
	IF @BookingTerminationTime Is NULL
		Set @RealEndTime = DateAdd(minute, @BookingServiceTime, @BookingEndTime);
	Else
		--if booking termination time more than start time then calculate real time from termination time
		-- else booking is canceled and we return 0 because this booking duration equal 0
		If @BookingTerminationTime > @BookingStartTime
			Set @RealEndTime = DateAdd(minute, @BookingServiceTime, @BookingTerminationTime);
		Else
			Return 0
	
	--if the user tries to book a resource that has previously booked 
	-- than check is a time-out went out to the this user reservation
	-- if timeout not went than return 1 (false)
	If @RequestUserID = @BookingUserID And DateAdd(minute, @BookingPerUserTimeout, @BookingEndTime) >= @RequestStartTime
		Return 1

	--check is this booking in required range if true then return 1 else 0
	IF (
		--start time <  requested start time < real end time
			@RequestStartTime BETWEEN @BookingStartTime AND @RealEndTime
		--start time < requested end time < real end time
		OR @RequestEndTime BETWEEN @BookingStartTime AND @RealEndTime 
		--requested start time < real end time < requested end time
		OR @RealEndTime BETWEEN @RequestEndTime AND @RequestStartTime
		--requested start time < start time < requested end time
		OR @BookingStartTime BETWEEN @RequestStartTime AND @RequestEndTime
	) 
		Return 1
	Else
		Return 0

	-- This is unreacheable code but SQL Cant verify control flow and this must be as dummy return
	Return 0
END
