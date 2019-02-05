-- ============================================================
-- Author:		Ratushnyiy Oleksiy
-- Create date: 05.02.2019
-- Description:	Check is booking range available,
--              return 1 if not applicable and 0 if applicable
-- ============================================================
CREATE PROCEDURE [dbo].[IsRangeAvailable]
	@RequestStartTime datetime,
	@RequestEndTime datetime,
	@BookingStartTime datetime,
	@BookingEndTime datetime,
	@BookingTerminationTime datetime,
	@BookingServiceTime int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
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
END
