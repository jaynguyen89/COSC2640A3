CREATE TABLE [dbo].[Enrolment]
(
	[Id] NVARCHAR(50) UNIQUE NOT NULL DEFAULT (NEWID()),
	[StudentId] NVARCHAR(50) NOT NULL,
	[ClassroomId] NVARCHAR(50) NOT NULL,
	[InvoiceId] NVARCHAR(50) DEFAULT NULL,
	[EnrolledOn] DATETIME2(7) NOT NULL DEFAULT (GETDATE()),
	[OverallMark] TINYINT NOT NULL DEFAULT 0,
	[IsPassed] BIT NOT NULL DEFAULT 0,
	CONSTRAINT [PK_Enrolment_Id] PRIMARY KEY ([Id] ASC),
	CONSTRAINT [FK_Enrolment_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Student] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_Enrolment_Invoice_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoice] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_Enrolment_Classroom_ClassroomId] FOREIGN KEY ([ClassroomId]) REFERENCES [dbo].[Classroom] ([Id])
)
