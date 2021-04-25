CREATE TABLE [dbo].[Invoice]
(
	[Id] NVARCHAR(50) UNIQUE NOT NULL DEFAULT (NEWID()),
	[EnrolmentId] NVARCHAR(50) NOT NULL,
	[DueAmount] DECIMAL(10,2) NOT NULL DEFAULT 0,
	[IsPaid] BIT NOT NULL DEFAULT 0,
	[PaymentMethod] NVARCHAR(50) DEFAULT NULL,
	[PaymentId] NVARCHAR(50) DEFAULT NULL, --For Paypal & Card payments
	[TransactionId] NVARCHAR(50) DEFAULT NULL, --For GooglePay & AliPay
	[ChargeId] NVARCHAR(50) DEFAULT NULL, --For GooglePay & AliPay
	[PaymentStatus] NVARCHAR(50) DEFAULT NULL,
	[PaidOn] DATETIME2(7) DEFAULT NULL,
	CONSTRAINT [PK_Invoice_Id] PRIMARY KEY ([Id] ASC),
	CONSTRAINT [FK_Invoice_Enrolment_EnrolmentId] FOREIGN KEY ([EnrolmentId]) REFERENCES [dbo].[Enrolment] ([Id]) ON DELETE CASCADE
)
