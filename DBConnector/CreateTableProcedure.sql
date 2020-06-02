USE [C:\USERS\OMANT\MONETARYMANAGEMENT.MDF]
GO
/****** Object:  StoredProcedure [dbo].[CreateMonthlyTable]    Script Date: 2020/06/02 23:38:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CreateMonthlyTable]
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
