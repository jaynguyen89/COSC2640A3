SELECT * FROM [dbo].Account;
SELECT * FROM [dbo].Student;
SELECT * FROM [dbo].Teacher;

SELECT * FROM [dbo].Classroom;
SELECT * FROM [dbo].Enrolment;
SELECT * FROM [dbo].Invoice;

update Invoice set IsPaid = '0', PaymentMethod = null, PaymentId = null, TransactionId = null, ChargeId = null, PaymentStatus = null, PaidOn = null;

--delete from Invoice;
--delete from Enrolment;
--delete from Classroom;
--delete from Student;
--delete from Teacher;
--delete from AccountRole;
--delete from Account;

select count(*) from Invoice;
select count(*) from Enrolment;
select count(*) from Classroom;
select count(*) from Student;
select count(*) from Teacher;
select count(*) from AccountRole;
select count(*) from Account;