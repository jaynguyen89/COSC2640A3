CREATE TABLE [dbo].[ClassContent]
(
	[Id] NVARCHAR(50) UNIQUE NOT NULL DEFAULT (NEWID()),
	[ClassroomId] NVARCHAR(50) NOT NULL,
	[Videos] NVARCHAR(4000) DEFAULT NULL, -- Save JSON array: [{ fileId, fileName, fileType, uploadedOn }]
	[Audios] NVARCHAR(4000) DEFAULT NULL, -- Save JSON array: [{ fileId, fileName, fileType, uploadedOn }]
	[Photos] NVARCHAR(4000) DEFAULT NULL, -- Save JSON array: [{ fileId, fileName, fileType, uploadedOn }]
	[Attachments] NVARCHAR(4000) DEFAULT NULL, -- Save JSON array: [{ fileId, fileName, fileType, uploadedOn }]
	[HtmlContent] NVARCHAR(MAX) DEFAULT NULL,
	CONSTRAINT [PK_ClassContent_Id] PRIMARY KEY ([Id] ASC),
	CONSTRAINT [FK_ClassContent_Classroom_ClassroomId] FOREIGN KEY ([ClassroomId]) REFERENCES [dbo].[Classroom] ([Id]) ON DELETE CASCADE
)
