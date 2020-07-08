CREATE PROCEDURE [dbo].[ReturnSumMonthlyPrice](
	@targetYear int,@targetMonth int,@Result DECIMAL OUTPUT
)
AS
BEGIN
	DECLARE @MainQuery nvarchar(max);
	DECLARE @FROMSentence nvarchar(max);
	DECLARE @OutParamSentence nvarchar(max);
	DECLARE @TableName nvarchar(max);
	SET @TableName=CONCAT(@targetYear,'-',FORMAT(@targetMonth,'00'))
	SET @MainQuery = CONCAT('SELECT @OutResult = SUM([Price]) FROM [dbo].[',@TableName,']');
	SET @OutParamSentence = '@OutResult DECIMAL OUTPUT';
    

	EXECUTE sp_executesql @MainQuery,@OutParamSentence,@OutResult=@Result OUTPUT;
	RETURN
END
GO


