CREATE TABLE [dbo].[DataCache]
(
	[Id] NVARCHAR(50) UNIQUE NOT NULL DEFAULT (NEWID()),
	[DataType] NVARCHAR(50) DEFAULT NULL,
	[DataId] NVARCHAR(50) DEFAULT NULL,
	[DataKey] NVARCHAR(50) DEFAULT NULL,
	[SearchInput] NVARCHAR(1000) DEFAULT NULL,
	[SerializedData] NVARCHAR(MAX) NOT NULL,
    [Timestamp] BIGINT NOT NULL,
	CONSTRAINT [PK_DataCache_Id] PRIMARY KEY ([Id] ASC)
)