--CREATE DATABASE
CREATE DATABASE [Document]

USE [Document]
GO

/****** Object:  Table [dbo].[Details]    Script Date: 09/11/2026 7:13:45 am ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--CREATE TABLE
CREATE TABLE [dbo].[Details](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OriginalFileName] [nvarchar](max) NOT NULL,
	[StoredIdentifier] [nvarchar](max) NOT NULL,
	[ContentType] [nvarchar](10) NOT NULL,
	[FileSize] [bigint] NOT NULL,
	[DateAndTimeUploaded] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


--CREATE STORED PROCEDURE
CREATE PROCEDURE [dbo].[sp_DeleteDocumentById]
   @StoredIdentifier NVARCHAR(MAX) 
AS
BEGIN
   SET NOCOUNT ON;
   DELETE FROM Details
   OUTPUT DELETED.Id
   WHERE StoredIdentifier = @StoredIdentifier
END;

CREATE PROCEDURE [dbo].[sp_GetDocumentById]
   @StoredIdentifier NVARCHAR(MAX) 
AS
BEGIN
   SET NOCOUNT ON;
   SELECT * FROM Details
   WHERE StoredIdentifier = @StoredIdentifier
END;

CREATE PROCEDURE [dbo].[sp_GetDocuments]
AS
BEGIN
   SET NOCOUNT ON;
   SELECT * FROM Details
END;

CREATE PROCEDURE [dbo].[sp_UploadDocument]
    @OriginalFileName NVARCHAR(MAX) 
   ,@StoredIdentifier NVARCHAR(MAX) 
   ,@ContentType NVARCHAR(10) 
   ,@FileSize BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Details (
	 OriginalFileName 
	,StoredIdentifier 
	,ContentType 
	,FileSize 
	,DateAndTimeUploaded) 
    OUTPUT INSERTED.Id 
    VALUES (
	 @OriginalFileName 
	,@StoredIdentifier 
	,@ContentType 
	,@FileSize 
	,GETDATE())
END;