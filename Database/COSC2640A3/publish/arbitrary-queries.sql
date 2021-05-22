SELECT * FROM [dbo].Account;
SELECT * FROM [dbo].Student;
SELECT * FROM [dbo].Teacher;

SELECT * FROM [dbo].Classroom;
SELECT * FROM [dbo].Enrolment;
SELECT * FROM [dbo].Invoice;

update Invoice set IsPaid = '0', PaymentMethod = null, PaymentId = null, TransactionId = null, ChargeId = null, PaymentStatus = null, PaidOn = null;