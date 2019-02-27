-- Determines the occupancy percents for the specified resource.
CREATE PROCEDURE [dbo].[Resource.OccupancyPercents]	@resourceId int
AS
BEGIN
	-- Check resource existence
	if NOT EXISTS (SELECT 1 FROM [Resources] WHERE [Id] = @resourceId)
		Throw 50001, 'Current entry (Resource) not found.',  12;

	-- Determining basic rule properties
	DECLARE @serviceMinutes int;
	DECLARE @preOrderTimeLimit int;
	DECLARE @maxTime int;
	DECLARE @ruleId int;

	SELECT TOP(1) 
		@ruleId = [Rule].[Id],
		@preOrderTimeLimit = [Rule].[PreOrderTimeLimit],
		@maxTime = [Rule].[MaxTime],
		@serviceMinutes = COALESCE([Rule].[ServiceTime],0)
	FROM [Resources]
	INNER JOIN [Rules] AS [Rule] ON [Resources].[RuleId] = [Rule].[Id]
	WHERE [Resources].[Id] = @resourceId

	if @ruleId IS NULL
		Throw 50001, 'Related entry (Rule) not found.',  2;

	if @preOrderTimeLimit IS NULL
		Throw 50001, 'Absurd Field Value: Preorder Limit is NULL.',  16;

	if @maxTime IS NULL
		Throw 50001, 'Absurd Field Value: Max Time is NULL.',  16;

	-- Zero preorder limit means available timeframe is infinity, thus occupancy is undefined
	if @preOrderTimeLimit = 0
		 return -1

	DECLARE @now datetime2 = GETDATE()

	-- Calculate mutual duration of all applicable booking ranges.
	DECLARE @sumDuration int = (
		SELECT SUM(BookingDuration + @serviceMinutes)
		FROM
			(SELECT 
				CASE
					WHEN [TerminationTime] IS NOT NULL AND [TerminationTime] > [EndTime]
						THEN 1 
						ELSE 0 
				END AS IsWierd,

				DATEDIFF(
					minute,
	 				CASE
					WHEN [StartTime] > @now
						THEN [StartTime] 
						ELSE @now
					END,
					COALESCE([TerminationTime], [EndTime])
				) AS BookingDuration

			FROM [Bookings]
			WHERE [ResourceId] = @resourceId
			) AS MidResult

		WHERE BookingDuration > 0 AND IsWierd = 0  
	)

	-- No apt bookings means resource is free.
	if @sumDuration IS NULL
		 return 0
	
	return 100 * ( CAST(@sumDuration AS float) / CAST(@preOrderTimeLimit+@maxTime AS float) )
END