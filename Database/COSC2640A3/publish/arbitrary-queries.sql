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

select Id from Account where Username = 'jay.nguyen';

select Id from Teacher where AccountId = 'be7018fc-93ec-44dd-bfb9-e8a6ec8dedd3';
select Id from Student where AccountId = 'be7018fc-93ec-44dd-bfb9-e8a6ec8dedd3';

delete from AccountRole where AccountId = 'be7018fc-93ec-44dd-bfb9-e8a6ec8dedd3';

delete from Teacher where AccountId = 'be7018fc-93ec-44dd-bfb9-e8a6ec8dedd3';
delete from Student where AccountId = 'be7018fc-93ec-44dd-bfb9-e8a6ec8dedd3';

delete from AccountRole where AccountId = 'be7018fc-93ec-44dd-bfb9-e8a6ec8dedd3';

delete from Classroom where TeacherId = '4522F089-940A-43B2-9904-6BB64612C292';

delete from Invoice where Id in (select I.Id from Student S, Enrolment E, Invoice I
where S.Id = E.StudentId and E.InvoiceId = I.Id and S.Id = '24BD2F63-A350-4DAB-9D13-6EF8CA4EA40B');

select * from Classroom where TeacherId = '4522F089-940A-43B2-9904-6BB64612C292';

select * from Enrolment where ClassroomId in (select Id from Classroom where TeacherId = '4522F089-940A-43B2-9904-6BB64612C292');

delete from Enrolment where ClassroomId = '29003175-AAD4-46F9-B81B-B3AA8762E21C';

delete from Account where Id = 'be7018fc-93ec-44dd-bfb9-e8a6ec8dedd3';

select * from Account where username = 'jay.nguyen'; --67d5f965-98a9-4dc9-ab6a-c865429d2120

select * from Student where AccountId = '67d5f965-98a9-4dc9-ab6a-c865429d2120'; -- DB53FE49-1EE6-43BF-AD1E-42EC9CC01E52
select * from Teacher where AccountId = '67d5f965-98a9-4dc9-ab6a-c865429d2120';

select * from DataCache;

delete from DataCache;

select * from Classroom where TeacherId = '29756E52-ECC1-414A-A45B-262D382970AA';

delete from Classroom where Id = 'FE31E94A-AB87-49E8-966C-4BED0BAD42F8';

select * from Enrolment where StudentId = 'DB53FE49-1EE6-43BF-AD1E-42EC9CC01E52';

select * from Invoice where Id = '2F8C31C0-6AAC-4715-B710-2F2C18B247FE';