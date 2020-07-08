CREATE PROCEDURE [dbo].[GetMonthFirstBalance](
	@targetYear int,@targetMonth int, @Result DECIMAL OUTPUT
)
AS
BEGIN
	IF([dbo].[IsExistBalance](@targetYear,@targetMonth) =0)
	--指定月の月初残額が見つからない場合
	BEGIN

		--計算後の差額を引数で受け取った指定年月の残額として挿入
		EXECUTE [dbo].[InsertMonthlyFirstBalance] @targetYear,@targetMonth


	END

		SELECT TOP 1 @Result = [Price]
		FROM [dbo].[MonthlyFund]
		WHERE [Year]=@targetYear
		AND [Month]=@targetMonth
		ORDER BY [dbo].[MonthlyFund].ID DESC

END
GO


