CREATE PROCEDURE [dbo].[InsertMonthlyFirstBalance](
	@targetYear int,@targetMonth int
)
AS
BEGIN
		--前年月情報の算出
		DECLARE @prevYear INT, @prevMonth INT

		IF(@targetMonth<=1)
		--指定月が1月だった場合
		BEGIN
			SET @prevYear = @targetYear-1
			SET @prevMonth = 12
		END
		ELSE
		--指定月が1でない場合
		BEGIN
			SET @prevYear = @targetYear
			SET @prevMonth=@targetMonth-1
		END
		

		--前月分の利用額総和を取得する
		DECLARE @MonthSum DECIMAL
		EXECUTE [dbo].[ReturnSumMonthlyPrice] @prevYear,@prevMonth,@Result=@MonthSum Output
		
		--前月分の月初残額を取得する
		DECLARE @MonthBalance DECIMAL
		SELECT @MonthBalance=Price
		FROM [dbo].[MonthlyFund]
		WHERE [Year]=@prevYear AND [Month]=@prevMonth

		--差額を計算する
		DECLARE @SubValue DECIMAL
		SET @SubValue = @MonthBalance - @MonthSum

		INSERT INTO [dbo].[MonthlyFund]([Year],[Month],[Price]) VALUES (@targetYear,@targetMonth,@SubValue)
END
GO


