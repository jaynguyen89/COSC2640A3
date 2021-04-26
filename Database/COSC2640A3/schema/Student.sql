﻿CREATE TABLE [dbo].[Student]
(
	[Id] NVARCHAR(50) UNIQUE NOT NULL DEFAULT (NEWID()),
	[AccountId] NVARCHAR(50) NOT NULL,
	[SchoolName] NVARCHAR(50) DEFAULT NULL,
	[Faculty] NVARCHAR(50) DEFAULT NULL,
	[PersonalUrl] NVARCHAR(100) DEFAULT NULL,
	CONSTRAINT [PK_Student_Id] PRIMARY KEY ([Id] ASC),
	CONSTRAINT [FK_Student_Account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]) ON DELETE CASCADE
)