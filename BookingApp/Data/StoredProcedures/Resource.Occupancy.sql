CREATE PROCEDURE [dbo].[Resource.Occupancy]
	@resourceId int
AS
BEGIN
	if NOT EXISTS (SELECT 1 FROM [Resources] WHERE [ResourceId] = @resourceId)
		Throw 50001, 'Current entry (Resource) not found.',  1;

	DECLARE @serviceMinutes int;
	DECLARE @preOrderTimeLimit int;
	DECLARE @ruleId int;

	SELECT TOP(1) 
		@ruleId = [Rule].[RuleId],
		@preOrderTimeLimit = [Rule].[PreOrderTimeLimit],
		@serviceMinutes = COALESCE([Rule].[ServiceTime],0)
	FROM [Resources]
	INNER JOIN [Rules] AS [Rule] ON [Resources].[RuleId] = [Rule].[RuleId]
	WHERE [Resources].[ResourceId] = @resourceId

	if @ruleId IS NULL
		Throw 50001, 'Related entry (Rule) not found.',  2;

	if @preOrderTimeLimit IS NULL
		Throw 50001, 'Absurd Field Value: Preorder Limit is NULL.',  3;


	DECLARE @now datetime2 = GETDATE()


	-- Zero preorder limit means available timeframe is infinity, so resource is virtually free.
	if @preOrderTimeLimit = 0
	BEGIN
		SELECT 
			*, CAST(0 AS float) as Occupancy
			FROM [Resources]
			WHERE [ResourceId] = @resourceId
		Return
	END



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

	-- Not found meaninful bookings durations means resource is free.
	if @sumDuration IS NULL
	BEGIN
		SELECT
			*, CAST(0 AS float) as Occupancy
			FROM [Resources]
			WHERE [ResourceId] = @resourceId
		Return
	END
	
	SELECT 
		*, CAST(@sumDuration AS float) / @preOrderTimeLimit as Occupancy
		FROM [Resources]
		WHERE [ResourceId] = @resourceId
		

END