CREATE FUNCTION [dbo].[IsExistBalance](
	@targetYear int ,@targetMonth int
) 
RETURNS BIT
AS
BEGIN
	DECLARE @ResultFlag bit, @SearchResult int;

	SELECT TOP 1 @SearchResult = [Price]
	FROM [dbo].[MonthlyFund]
	WHERE [Year]=@targetYear
	AND [Month]=@targetMonth
	ORDER BY [dbo].[MonthlyFund].ID DESC

	IF @SearchResult IS NOT NULL
	BEGIN
		SET @ResultFlag=1
	END
	ELSE
	BEGIN
		SET @ResultFlag=0
	END

	RETURN @ResultFlag;

END;
GO


