CREATE TABLE [dbo].[StudentMark]
(
	[Id] NVARCHAR(50) UNIQUE NOT NULL DEFAULT (NEWID()),
	[EnrolmentId] NVARCHAR(50) NOT NULL,
	[AssessmentId] NVARCHAR(50) NOT NULL,
	[Marks] TINYINT NOT NULL DEFAULT 0,
	[MarkedOn] DATETIME2(7) NOT NULL DEFAULT (GETDATE()),
	[Comment] NVARCHAR(250) DEFAULT NULL,
	CONSTRAINT [PK_StudentMark_Id] PRIMARY KEY ([Id] ASC),
	CONSTRAINT [FK_StudentMark_Enrolment_EnrolmentId] FOREIGN KEY ([EnrolmentId]) REFERENCES [dbo].[Enrolment] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_StudentMark_Assessment_AssessmentId] FOREIGN KEY ([AssessmentId]) REFERENCES [dbo].[Assessment] ([Id])
)
