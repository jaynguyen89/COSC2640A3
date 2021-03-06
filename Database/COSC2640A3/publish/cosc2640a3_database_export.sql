USE [COSC2640A3]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 22/05/2021 2:11:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[Id] [nvarchar](50) NOT NULL,
	[EmailAddress] [nvarchar](100) NOT NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[NormalizedUsername] [nvarchar](50) NOT NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[TwoFaSecretKey] [nvarchar](20) NULL,
	[RecoveryToken] [nvarchar](100) NULL,
	[TokenSetOn] [datetime2](7) NULL,
	[PreferredName] [nvarchar](100) NULL,
 CONSTRAINT [PK_Account_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccountRole]    Script Date: 22/05/2021 2:11:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountRole](
	[Id] [nvarchar](50) NOT NULL,
	[AccountId] [nvarchar](50) NOT NULL,
	[Role] [tinyint] NOT NULL,
 CONSTRAINT [PK_AccountRole_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClassContent]    Script Date: 22/05/2021 2:11:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClassContent](
	[Id] [nvarchar](50) NOT NULL,
	[ClassroomId] [nvarchar](50) NOT NULL,
	[Videos] [nvarchar](4000) NULL,
	[Audios] [nvarchar](4000) NULL,
	[Photos] [nvarchar](4000) NULL,
	[Attachments] [nvarchar](4000) NULL,
	[HtmlContent] [nvarchar](max) NULL,
 CONSTRAINT [PK_ClassContent_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Classroom]    Script Date: 22/05/2021 2:11:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classroom](
	[Id] [nvarchar](50) NOT NULL,
	[TeacherId] [nvarchar](50) NOT NULL,
	[ClassName] [nvarchar](70) NULL,
	[Capacity] [smallint] NOT NULL,
	[Price] [decimal](6, 2) NOT NULL,
	[CommencedOn] [datetime2](7) NULL,
	[Duration] [tinyint] NOT NULL,
	[DurationUnit] [tinyint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Classroom_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Enrolment]    Script Date: 22/05/2021 2:11:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Enrolment](
	[Id] [nvarchar](50) NOT NULL,
	[StudentId] [nvarchar](50) NOT NULL,
	[ClassroomId] [nvarchar](50) NOT NULL,
	[InvoiceId] [nvarchar](50) NULL,
	[EnrolledOn] [datetime2](7) NOT NULL,
	[OverallMark] [tinyint] NULL,
	[MarkBreakdowns] [nvarchar](max) NULL,
	[IsPassed] [bit] NULL,
 CONSTRAINT [PK_Enrolment_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoice]    Script Date: 22/05/2021 2:11:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoice](
	[Id] [nvarchar](50) NOT NULL,
	[DueAmount] [decimal](10, 2) NOT NULL,
	[IsPaid] [bit] NOT NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[PaymentId] [nvarchar](50) NULL,
	[TransactionId] [nvarchar](50) NULL,
	[ChargeId] [nvarchar](50) NULL,
	[PaymentStatus] [nvarchar](50) NULL,
	[PaidOn] [datetime2](7) NULL,
 CONSTRAINT [PK_Invoice_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Student]    Script Date: 22/05/2021 2:11:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Student](
	[Id] [nvarchar](50) NOT NULL,
	[AccountId] [nvarchar](50) NOT NULL,
	[SchoolName] [nvarchar](50) NULL,
	[Faculty] [nvarchar](50) NULL,
	[PersonalUrl] [nvarchar](100) NULL,
 CONSTRAINT [PK_Student_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Teacher]    Script Date: 22/05/2021 2:11:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teacher](
	[Id] [nvarchar](50) NOT NULL,
	[AccountId] [nvarchar](50) NOT NULL,
	[Company] [nvarchar](50) NULL,
	[JobTitle] [nvarchar](20) NULL,
	[PersonalWebsite] [nvarchar](100) NULL,
 CONSTRAINT [PK_Teacher_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT ((0)) FOR [EmailConfirmed]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (NULL) FOR [PhoneNumber]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT ((0)) FOR [PhoneNumberConfirmed]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT ((0)) FOR [TwoFactorEnabled]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (NULL) FOR [TwoFaSecretKey]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (NULL) FOR [RecoveryToken]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (NULL) FOR [TokenSetOn]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (NULL) FOR [PreferredName]
GO
ALTER TABLE [dbo].[AccountRole] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ClassContent] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ClassContent] ADD  DEFAULT (NULL) FOR [Videos]
GO
ALTER TABLE [dbo].[ClassContent] ADD  DEFAULT (NULL) FOR [Audios]
GO
ALTER TABLE [dbo].[ClassContent] ADD  DEFAULT (NULL) FOR [Photos]
GO
ALTER TABLE [dbo].[ClassContent] ADD  DEFAULT (NULL) FOR [Attachments]
GO
ALTER TABLE [dbo].[ClassContent] ADD  DEFAULT (NULL) FOR [HtmlContent]
GO
ALTER TABLE [dbo].[Classroom] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Classroom] ADD  DEFAULT (NULL) FOR [ClassName]
GO
ALTER TABLE [dbo].[Classroom] ADD  DEFAULT ((1)) FOR [Capacity]
GO
ALTER TABLE [dbo].[Classroom] ADD  DEFAULT ((1)) FOR [Price]
GO
ALTER TABLE [dbo].[Classroom] ADD  DEFAULT (NULL) FOR [CommencedOn]
GO
ALTER TABLE [dbo].[Classroom] ADD  DEFAULT ((1)) FOR [Duration]
GO
ALTER TABLE [dbo].[Classroom] ADD  DEFAULT ((0)) FOR [DurationUnit]
GO
ALTER TABLE [dbo].[Classroom] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Classroom] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Enrolment] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Enrolment] ADD  DEFAULT (NULL) FOR [InvoiceId]
GO
ALTER TABLE [dbo].[Enrolment] ADD  DEFAULT (getdate()) FOR [EnrolledOn]
GO
ALTER TABLE [dbo].[Enrolment] ADD  DEFAULT (NULL) FOR [OverallMark]
GO
ALTER TABLE [dbo].[Enrolment] ADD  DEFAULT (NULL) FOR [MarkBreakdowns]
GO
ALTER TABLE [dbo].[Enrolment] ADD  DEFAULT (NULL) FOR [IsPassed]
GO
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT ((0)) FOR [DueAmount]
GO
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT ((0)) FOR [IsPaid]
GO
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT (NULL) FOR [PaymentMethod]
GO
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT (NULL) FOR [PaymentId]
GO
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT (NULL) FOR [TransactionId]
GO
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT (NULL) FOR [ChargeId]
GO
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT (NULL) FOR [PaymentStatus]
GO
ALTER TABLE [dbo].[Invoice] ADD  DEFAULT (NULL) FOR [PaidOn]
GO
ALTER TABLE [dbo].[Student] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Student] ADD  DEFAULT (NULL) FOR [SchoolName]
GO
ALTER TABLE [dbo].[Student] ADD  DEFAULT (NULL) FOR [Faculty]
GO
ALTER TABLE [dbo].[Student] ADD  DEFAULT (NULL) FOR [PersonalUrl]
GO
ALTER TABLE [dbo].[Teacher] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Teacher] ADD  DEFAULT (NULL) FOR [Company]
GO
ALTER TABLE [dbo].[Teacher] ADD  DEFAULT (NULL) FOR [JobTitle]
GO
ALTER TABLE [dbo].[Teacher] ADD  DEFAULT (NULL) FOR [PersonalWebsite]
GO
ALTER TABLE [dbo].[AccountRole]  WITH CHECK ADD  CONSTRAINT [FK_AccountRole_Account_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Account] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AccountRole] CHECK CONSTRAINT [FK_AccountRole_Account_AccountId]
GO
ALTER TABLE [dbo].[ClassContent]  WITH CHECK ADD  CONSTRAINT [FK_ClassContent_Classroom_ClassroomId] FOREIGN KEY([ClassroomId])
REFERENCES [dbo].[Classroom] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ClassContent] CHECK CONSTRAINT [FK_ClassContent_Classroom_ClassroomId]
GO
ALTER TABLE [dbo].[Classroom]  WITH NOCHECK ADD  CONSTRAINT [FK_Classroom_Teacher_TeacherId] FOREIGN KEY([TeacherId])
REFERENCES [dbo].[Teacher] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Classroom] CHECK CONSTRAINT [FK_Classroom_Teacher_TeacherId]
GO
ALTER TABLE [dbo].[Enrolment]  WITH CHECK ADD  CONSTRAINT [FK_Enrolment_Classroom_ClassroomId] FOREIGN KEY([ClassroomId])
REFERENCES [dbo].[Classroom] ([Id])
GO
ALTER TABLE [dbo].[Enrolment] CHECK CONSTRAINT [FK_Enrolment_Classroom_ClassroomId]
GO
ALTER TABLE [dbo].[Enrolment]  WITH CHECK ADD  CONSTRAINT [FK_Enrolment_Invoice_InvoiceId] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Enrolment] CHECK CONSTRAINT [FK_Enrolment_Invoice_InvoiceId]
GO
ALTER TABLE [dbo].[Enrolment]  WITH CHECK ADD  CONSTRAINT [FK_Enrolment_Student_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Enrolment] CHECK CONSTRAINT [FK_Enrolment_Student_StudentId]
GO
ALTER TABLE [dbo].[Student]  WITH CHECK ADD  CONSTRAINT [FK_Student_Account_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Account] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Student] CHECK CONSTRAINT [FK_Student_Account_AccountId]
GO
ALTER TABLE [dbo].[Teacher]  WITH CHECK ADD  CONSTRAINT [FK_Teacher_Account_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Account] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Teacher] CHECK CONSTRAINT [FK_Teacher_Account_AccountId]
GO
