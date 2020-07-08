CREATE PROCEDURE [dbo].[InsertMonthlyFirstBalance](
	@targetYear int,@targetMonth int
)
AS
BEGIN
		--�O�N�����̎Z�o
		DECLARE @prevYear INT, @prevMonth INT

		IF(@targetMonth<=1)
		--�w�茎��1���������ꍇ
		BEGIN
			SET @prevYear = @targetYear-1
			SET @prevMonth = 12
		END
		ELSE
		--�w�茎��1�łȂ��ꍇ
		BEGIN
			SET @prevYear = @targetYear
			SET @prevMonth=@targetMonth-1
		END
		

		--�O�����̗��p�z���a���擾����
		DECLARE @MonthSum DECIMAL
		EXECUTE [dbo].[ReturnSumMonthlyPrice] @prevYear,@prevMonth,@Result=@MonthSum Output
		
		--�O�����̌����c�z���擾����
		DECLARE @MonthBalance DECIMAL
		SELECT @MonthBalance=Price
		FROM [dbo].[MonthlyFund]
		WHERE [Year]=@prevYear AND [Month]=@prevMonth

		--���z���v�Z����
		DECLARE @SubValue DECIMAL
		SET @SubValue = @MonthBalance - @MonthSum

		INSERT INTO [dbo].[MonthlyFund]([Year],[Month],[Price]) VALUES (@targetYear,@targetMonth,@SubValue)
END
GO


