SELECT * FROM [dbo].Account;
SELECT * FROM [dbo].Student;
SELECT * FROM [dbo].Teacher;

SELECT * FROM [dbo].Classroom;
SELECT * FROM [dbo].Enrolment;
SELECT * FROM [dbo].Invoice;

update Invoice set IsPaid = '0', PaymentMethod = null, PaymentId = null, TransactionId = null, ChargeId = null, PaymentStatus = null, PaidOn = null;

SELECT COUNT(*) FROM [dbo].Account;
SELECT COUNT(*) FROM [dbo].Student;
SELECT COUNT(*) FROM [dbo].Teacher;

SELECT COUNT(*) FROM [dbo].Classroom;
SELECT COUNT(*) FROM [dbo].Enrolment;
SELECT COUNT(*) FROM [dbo].Invoice;

delete from Invoice;
delete from Enrolment;
delete from Classroom;
delete from Student;
delete from Teacher;
delete from AccountRole;
delete from Account;