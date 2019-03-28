-- ==========================================================
-- Author:		Kukulyak Taras
-- Create date: 22.03.2019
-- Description: Checking Is Parent Valid
-- ==========================================================
CREATE PROCEDURE [dbo].[Folders.IsParentValid]
	@NewParentID INT,
	@CurrentID INT
AS
BEGIN
	DECLARE @CurrentParentID INT;
	DECLARE @Res INT = 1;

	SET @CurrentParentID = @NewParentID;
	
	WHILE @Res = 1
		BEGIN
		IF @CurrentParentID < 1
			BREAK;
		ELSE
			IF @CurrentParentID = @CurrentID
				Throw 50000, 'Specified parent Folder cant be set because this would cause circluar dependency', 547;
			ELSE
				if not exists (SELECT ParentFolderId FROM folders WHERE folders.Id = @CurrentParentID)
					Throw 50001, 'Specified Folder not found', 12;
				(SELECT 
					@CurrentParentID = ISNULL(ParentFolderId, 0)
				FROM folders 
				WHERE folders.Id = @CurrentParentID
				);
		END
END