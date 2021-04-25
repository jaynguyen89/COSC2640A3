CREATE TABLE [dbo].[StudentMark]
(
	[Id] NVARCHAR(50) UNIQUE NOT NULL DEFAULT (NEWID()),
	[EnrolmentId] NVARCHAR(50) NOT NULL,
	[MarkBreakdowns] TINYINT NOT NULL DEFAULT 0, --Save JSON: AssessmentName, TotalMarks, StudentMarks, MarkedOn, Note
	[Comment] NVARCHAR(250) DEFAULT NULL,
	CONSTRAINT [PK_StudentMark_Id] PRIMARY KEY ([Id] ASC),
	CONSTRAINT [FK_StudentMark_Enrolment_EnrolmentId] FOREIGN KEY ([EnrolmentId]) REFERENCES [dbo].[Enrolment] ([Id]) ON DELETE CASCADE
)
