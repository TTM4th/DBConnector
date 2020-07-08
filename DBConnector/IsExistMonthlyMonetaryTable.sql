CREATE FUNCTION [dbo].[IsExistMonthlyMonetaryTable](
	@targetYear int ,@targetMonth int
)
RETURNS BIT
AS
BEGIN
	DECLARE @ResultFlag bit,@TableName as nvarchar(max);
	SET @TableName=CONCAT(@targetYear,'-',FORMAT(@targetMonth,'00'));
	
	IF EXISTS (SELECT * From INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName)
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


