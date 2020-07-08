CREATE PROCEDURE [dbo].[CreateMonthlyTable]
	 @TableName nvarchar(10)
As
BEGIN
DECLARE @Query nvarchar(max)
	SET @Query = CONCAT('Create Table [',@TableName,'] (
	ID int IDENTITY(1,1) NOT NULL,
	[Date] date,
	Price decimal(28, 0),
	Classification char(1)
	)')
EXEC (@Query)
END
GO


