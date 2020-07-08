CREATE PROCEDURE [dbo].[GetMonthFirstBalance](
	@targetYear int,@targetMonth int, @Result DECIMAL OUTPUT
)
AS
BEGIN
	IF([dbo].[IsExistBalance](@targetYear,@targetMonth) =0)
	--�w�茎�̌����c�z��������Ȃ��ꍇ
	BEGIN

		--�v�Z��̍��z�������Ŏ󂯎�����w��N���̎c�z�Ƃ��đ}��
		EXECUTE [dbo].[InsertMonthlyFirstBalance] @targetYear,@targetMonth


	END

		SELECT TOP 1 @Result = [Price]
		FROM [dbo].[MonthlyFund]
		WHERE [Year]=@targetYear
		AND [Month]=@targetMonth
		ORDER BY [dbo].[MonthlyFund].ID DESC

END
GO


