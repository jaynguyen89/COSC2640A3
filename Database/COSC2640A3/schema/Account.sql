﻿CREATE TABLE [dbo].[Account]
(
	[Id] NVARCHAR(50) UNIQUE NOT NULL DEFAULT (NEWID()),
	[EmailAddress] NVARCHAR(100) NOT NULL,
	[EmailConfirmed] BIT NOT NULL DEFAULT 0,
	[Username] NVARCHAR(50) NOT NULL,
	[NormalizedUsername] NVARCHAR(50) NOT NULL,
	[PhoneNumber] NVARCHAR(20) DEFAULT NULL,
	[PhoneNumberConfirmed] BIT NOT NULL DEFAULT 0,
	[TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
	[TwoFaSecretKey] NVARCHAR(20) DEFAULT NULL,
	[RecoveryToken] NVARCHAR(100) DEFAULT NULL,
	[TokenSetOn] DATETIME2(7) DEFAULT NULL,
	[PreferredName] NVARCHAR(100) DEFAULT NULL,
	CONSTRAINT [PK_Account_Id] PRIMARY KEY ([Id] ASC)
);
