using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AmazonLibrary.Interfaces;
using COSC2640A3.Bindings;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using COSC2640A3.ViewModels.Features;
using Helper;
using Helper.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Helper.Shared.SharedConstants;

namespace COSC2640A3.Controllers {

    [ApiController]
    [Route("data")]
    public sealed class DataGenerator : ControllerBase {

        private readonly ILogger<DataGenerator> _logger;
        private readonly IGeneratorService _generatorService;
        private readonly IClassroomService _classroomService;
        private readonly IEnrolmentService _enrolmentService;
        private readonly IEmrService _emrService;
        private readonly IDynamoService _dynamoService;
        
        private const int NumberOfTeachers = 5000;
        private const int MinNumberOfClassroomsPerTeacher = 7;
        private const int MaxNumberOfClassroomsPerTeacher = 15;
        private const int NumberOfStudents = 100000;
        private const int MinPrice = 1000;
        private const int MaxPrice = 150000;

        public DataGenerator(
            ILogger<DataGenerator> logger,
            IGeneratorService generatorService,
            IClassroomService classroomService,
            IEnrolmentService enrolmentService,
            IEmrService emrService,
            IDynamoService dynamoService
        ) {
            _logger = logger;
            _generatorService = generatorService;
            _classroomService = classroomService;
            _enrolmentService = enrolmentService;
            _emrService = emrService;
            _dynamoService = dynamoService;
        }

        /// <summary>
        /// For guest. To trigger AWS EMR Mapper inside EMR Master Cluster from client-side.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /data/trigger-emr-mapper
        /// </code>
        /// -->
        /// </remarks>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = boolean }</returns>
        /// <response code="200">The request was successfully processed.</response>
        [HttpGet("trigger-emr-mapper")]
        public async Task<JsonResult> TriggerEmrMapper() {
            _logger.LogInformation($"{ nameof(DataGenerator) }.{ nameof(TriggerEmrMapper) }: service starts.");
            
            var emrProgress = await _dynamoService.GetLastEmrProgress();
            if (emrProgress is null) return new JsonResult(new JsonResponse { Result = SharedEnums.RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." }});
            if (Helpers.IsProperString(emrProgress.Id) && !emrProgress.ReducerDone)
                return new JsonResult(new JsonResponse {
                Result = SharedEnums.RequestResult.Failed,
                    Messages = new [] {
                        emrProgress.MapperDone
                            ? "The EMR Mapper has done mapping step. To save computing resources, mapper won\'t run again. Please execute reducer step."
                            : "You have previously executed the EMR Mapper. It is still running. Please come back later to check progress and execute reducer step."
                    }
                });

            var result = _emrService.ExecuteCommandMapper();
            return new JsonResult(new JsonResponse { Result = SharedEnums.RequestResult.Success, Data = result });
        }
        
        /// <summary>
        /// For guest. To trigger AWS EMR Reducer inside EMR Master Cluster from client-side.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /data/trigger-emr-reducer
        /// </code>
        /// -->
        /// </remarks>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = boolean }</returns>
        /// <response code="200">The request was successfully processed.</response>
        [HttpGet("trigger-emr-reducer")]
        public async Task<JsonResult> TriggerEmrReducer() {
            _logger.LogInformation($"{ nameof(DataGenerator) }.{ nameof(TriggerEmrReducer) }: service starts.");
            
            var emrProgress = await _dynamoService.GetLastEmrProgress();
            if (emrProgress is null) return new JsonResult(new JsonResponse { Result = SharedEnums.RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." }});
            if (Helpers.IsProperString(emrProgress.Id) && !emrProgress.ReducerDone)
                if (!emrProgress.MapperDone) return new JsonResult(new JsonResponse {
                    Result = SharedEnums.RequestResult.Failed,
                    Messages = new [] { "You have previously executed the EMR Mapper. It is still running. Please come back later to check progress and execute reducer step." }
                });
            
            var result = _emrService.ExecuteCommandReducer();
            return new JsonResult(new JsonResponse { Result = SharedEnums.RequestResult.Success, Data = result });
        }
        
        /// <summary>
        /// For guest. To get the statistics results after the EMR Mapper-Reducer have both finished their processing jobs.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /data/get-statistics
        /// </code>
        /// -->
        ///
        /// Returned object signature:
        /// <!--
        /// <code>
        /// {
        ///     progress: {
        ///         id: string,
        ///         timestamp: datetime,
        ///         mapperDone: boolean,
        ///         reducerDone: boolean
        ///     },
        ///     statistics: [{
        ///         id: string,
        ///         timestamp: datetime,
        ///         range50: number,
        ///         range100: number,
        ///         range250: number,
        ///         range500: number,
        ///         range1000: number,
        ///         range1001: number
        ///     }]
        /// }
        /// </code>
        /// -->
        /// </remarks>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        [HttpGet("get-statistics")]
        public async Task<JsonResult> GetStatisticsResult() {
            _logger.LogInformation($"{ nameof(DataGenerator) }.{ nameof(GetStatisticsResult) }: service starts.");
            
            var emrProgress = await _dynamoService.GetLastEmrProgress();
            if (emrProgress is null) return new JsonResult(new JsonResponse { Result = SharedEnums.RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." }});

            var statistics = await _dynamoService.GetEmrStatistics();
            var statisticsVm = statistics
                               .Select(statistic => {
                                   var statisticVm = JsonConvert.DeserializeObject<EmrStatisticsVM>(statistic.Statistics) ?? new EmrStatisticsVM();
                                   statisticVm.Id = statistic.Id;
                                   statisticVm.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(statistic.Timestamp).UtcDateTime;
                                   return statisticVm;
                               })
                               .OrderByDescending(statistic => statistic.Timestamp)
                               .ToArray();
            
            return new JsonResult(new JsonResponse { Result = SharedEnums.RequestResult.Success, Data = new { Progress = (EmrProgressVM) emrProgress, Statistics = statisticsVm } });
        }

        /// <summary>
        /// For guest. To generate data for SQL Server database. This service generates teacher-related data: Account, AccountRole, Teacher, Student (not used), Classroom.
        /// Returns array of the generated classroom IDs.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /data/generate-teachers-and-classrooms
        /// </code>
        /// -->
        /// </remarks>
        /// <returns>JsonResponse object: { Result = 0|1, Data = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        [HttpGet("generate-teachers-and-classrooms")]
        public async Task<JsonResult> GenerateTeacherAccountsAndClassroomsData() {
            _logger.LogInformation($"{ nameof(DataGenerator) }.{ nameof(GenerateTeacherAccountsAndClassroomsData) }: service starts.");

            var teacherAccounts = new List<Account>();
            for (var i = 0; i < NumberOfTeachers; i++) teacherAccounts.Add(GenerateAccount());
            
            _logger.LogInformation($"Generated { NumberOfTeachers } accounts.");

            var accountIds = await _generatorService.InsertMultipleAccounts(teacherAccounts.ToArray());
            if (accountIds is null) return new JsonResult(0);
            
            var teacherIds = await SaveRoles<Teacher>(accountIds);
            if (teacherIds is null) return new JsonResult(0);
            _logger.LogInformation("Done inserting teacher accounts and roles.");

            var classrooms = teacherIds
                               .SelectMany(teacherId => {
                                   var numberOfClassrooms = Helpers.GetRandomNumberInRangeInclusive(MaxNumberOfClassroomsPerTeacher, MinNumberOfClassroomsPerTeacher);
                                   var classroomsByTeacher = new List<Classroom>();
                                   
                                   for (var i = 0; i < numberOfClassrooms; i++) classroomsByTeacher.Add(GenerateClassroom(teacherId));
                                   return classroomsByTeacher;
                               })
                               .ToArray();

            var classroomIds = await _generatorService.InsertMultipleClassrooms(classrooms);
            return new JsonResult(classroomIds);
        }
        
        /// <summary>
        /// For guest. To generate data for SQL Server database. This service generates student-related data: Account, AccountRole, Teacher (not used), Student, Enrolment, Invoice.
        /// Returns the generated INSERT statements for the enrolments in FileResult if successful.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     POST /data/generate-students-and-enrolments/{save}
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="classroomData">The generated classroom IDs taken from the stage of generating teachers.</param>
        /// <param name="saveToDbDirectly">
        /// To indicate if the generated data should be inserted directly to SQL Server database or returned in Response as the INSERT statements.
        /// Set `<c>saveToDbDirectly == 0</c>` to get the INSERT statements, set `<c>saveToDbDirectly == 1</c>` to insert directly (EXTREMELY time/resource-consuming, not recommended).
        /// </param>
        /// <returns>JsonResponse object: { Result = 0|1, Data = FileResult }</returns>
        /// <response code="200">The request was successfully processed.</response>
        [HttpPost("generate-students-and-enrolments/{saveToDbDirectly}")]
        public async Task<ActionResult> GenerateStudentAccountsAndEnrolmentsData([FromBody] DataExport classroomData,[FromRoute] int saveToDbDirectly) {
            _logger.LogInformation($"{ nameof(DataGenerator) }.{ nameof(GenerateStudentAccountsAndEnrolmentsData) }: service starts.");

            var studentAccounts = new List<Account>();
            for (var i = 0; i < NumberOfStudents; i++) {
                _logger.LogInformation($"Generating student #{ i + 1 } of { NumberOfStudents }.");
                studentAccounts.Add(GenerateAccount());
            }

            var accountIds = await _generatorService.InsertMultipleAccounts(studentAccounts.ToArray());
            if (accountIds is null) return new JsonResult(0);

            var studentIds = await SaveRoles<Student>(accountIds);
            if (studentIds is null) return new JsonResult(0);
            _logger.LogInformation("Done inserting student accounts and roles.");

            var classrooms = classroomData.ClassroomIds.Select(async classroomId => await _classroomService.GetClassroomById(classroomId)).Select(task => task.Result).ToArray();

            if (saveToDbDirectly == 0) {
                var statements = GenerateInsertStatements(classrooms, classrooms.Length, studentIds);
                
                var exportedFile = new MemoryStream();
                var writer = new StreamWriter(exportedFile);
                foreach (var statement in statements) await writer.WriteLineAsync(statement);
                
                await writer.FlushAsync();
                exportedFile.Position = 0;
                return File(exportedFile, MediaTypeNames.Text.Plain, $"statements.sql");
            }

            if (!await SaveEnrolments(classrooms, classrooms.Length, studentIds)) return new JsonResult(0);
            return new JsonResult(1);
        }

        private async Task<bool> SaveEnrolments(Classroom[] classrooms, int classroomsCount, string[] studentIds) {
            foreach (var classroom in classrooms) {
                var invoiceId = await SaveInvoice(classroom.Price);
                if (invoiceId is null) return false;
                
                var numberOfEnrolments = GetRandomNumberOfEnrolmentByPriceRange(classroom.Price, classroom.Capacity);
                var classroomEnrolments = new List<string>();

                var index = Array.FindIndex(classrooms, c => c.Id.Equals(classroom.Id)) + 1;
                var enrolments = new List<Enrolment>();
                for (var i = 0; i < numberOfEnrolments; i++) {
                    _logger.LogInformation($"Classroom #{ index } of { classroomsCount }: Enrolment #{ i + 1 } of { numberOfEnrolments }");
                    
                    var studentId = studentIds[Helpers.GetRandomNumberInRangeInclusive(studentIds.Length - 1)];
                    while (classroomEnrolments.Contains(studentId)) studentId = studentIds[Helpers.GetRandomNumberInRangeInclusive(studentIds.Length - 1)];

                    enrolments.Add(new Enrolment {
                        StudentId = studentId,
                        ClassroomId = classroom.Id,
                        InvoiceId = invoiceId,
                        EnrolledOn = Helpers.GetRandomDateTime(DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(60))
                    });
                    classroomEnrolments.Add(studentId);
                }
                
                var success = await _generatorService.InsertMultipleEnrolments(enrolments.ToArray());
                if (!success.HasValue || !success.Value) return false;
            }
            
            return true;
        }

        private string[] GenerateInsertStatements(Classroom[] classrooms, int classroomsCount, string[] studentIds) {
            var methods = new[] { "PayPal", "GooglePay", "Visa", "Master", "Amex" };

            var invoicesByClassroomIds = (from classroom in classrooms
                let isPaid = Helpers.GetRandomNumberInRangeInclusive(1) == 1
                select new {
                    ClassroomId = classroom.Id,
                    Invoice = new Invoice {
                        Id = Guid.NewGuid().ToString().ToLower(),
                        DueAmount = classroom.Price,
                        IsPaid = isPaid,
                        PaymentId = isPaid ? Guid.NewGuid().ToString() : null,
                        ChargeId = isPaid ? Guid.NewGuid().ToString() : null,
                        TransactionId = isPaid ? Guid.NewGuid().ToString() : null,
                        PaymentStatus = isPaid ? "COMPLETED" : null,
                        PaymentMethod = isPaid ? methods[Helpers.GetRandomNumberInRangeInclusive(methods.Length - 1)] : null,
                        PaidOn = Helpers.GetRandomDateTime(DateTime.UtcNow.AddDays(60), DateTime.UtcNow.AddDays(90))
                    }
                })
                .ToDictionary(z => z.ClassroomId, z => z.Invoice);

            var invoices = invoicesByClassroomIds.Select(z => z.Value).ToArray();
            var success = _generatorService.InsertMultipleInvoices(invoices);
            if (!success.HasValue || !success.Value) return null;

            var enrolments = new List<string>();
            foreach (var classroom in classrooms) {
                var numberOfEnrolments = GetRandomNumberOfEnrolmentByPriceRange(classroom.Price, classroom.Capacity);
                var classroomEnrolments = new List<string>();
                
                var index = Array.FindIndex(classrooms, c => c.Id.Equals(classroom.Id)) + 1;
                for (var i = 0; i < numberOfEnrolments; i++) {
                    _logger.LogInformation($"Classroom #{ index } of { classroomsCount }: Enrolment #{ i + 1 } of { numberOfEnrolments }");
                    
                    var studentId = studentIds[Helpers.GetRandomNumberInRangeInclusive(studentIds.Length - 1)];
                    while (classroomEnrolments.Contains(studentId)) studentId = studentIds[Helpers.GetRandomNumberInRangeInclusive(studentIds.Length - 1)];

                    var markBreakdowns = GenerateEnrolmentMarks();
                    var overallMarks = new StudentMarks { MarkBreakdowns = markBreakdowns }.CalculateOverallMarks();
                    
                    enrolments.Add($"INSERT INTO Enrolment (StudentId, ClassroomId, InvoiceId, EnrolledOn, OverallMark, MarkBreakdowns, IsPassed) " +
                                   $"VALUES ('{ studentId }', '{ classroom.Id }', '{ invoicesByClassroomIds[classroom.Id].Id }', " +
                                   $"'{Helpers.GetRandomDateTime(DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(60)):yyyy-MM-dd HH:mm:ss}', " +
                                   $"{ (overallMarks == -1 ? "null" : $"{ overallMarks }") }, " +
                                   $"{ (markBreakdowns == null ? "null" : $"'{ JsonConvert.SerializeObject(markBreakdowns) }'") }, null);"
                                );
                    classroomEnrolments.Add(studentId);
                }
            }

            return enrolments.ToArray();
        }

        private async Task<string> SaveInvoice(decimal amount) {
            var isPaid = Helpers.GetRandomNumberInRangeInclusive(1) == 1;
            var methods = new[] { "PayPal", "GooglePay", "Visa", "Master", "Amex" };
            
            return await _enrolmentService.InsertNewInvoice(new Invoice {
                DueAmount = amount,
                IsPaid = isPaid,
                PaymentId = isPaid ? Guid.NewGuid().ToString() : null,
                ChargeId = isPaid ? Guid.NewGuid().ToString() : null,
                TransactionId = isPaid ? Guid.NewGuid().ToString() : null,
                PaymentStatus = isPaid ? "COMPLETED" : null,
                PaymentMethod = isPaid ? methods[Helpers.GetRandomNumberInRangeInclusive(methods.Length - 1)] : null,
                PaidOn = Helpers.GetRandomDateTime(DateTime.UtcNow.AddDays(60), DateTime.UtcNow.AddDays(90))
            });
        }

        private async Task<string[]> SaveRoles<T>(string[] accountIds) {
            if (accountIds is null) return null;
            
            var accountRoles = accountIds
                               .Select(accountId => new AccountRole {
                                   AccountId = accountId,
                                   Role = (byte) SharedEnums.Role.Student
                               })
                               .Concat(
                                   accountIds.Select(accountId => new AccountRole {
                                       AccountId = accountId,
                                       Role = (byte) SharedEnums.Role.Teacher
                                   })
                               )
                               .ToArray();

            var result = await _generatorService.InsertMultipleRoles(accountRoles);
            if (!result.HasValue || !result.Value) return null;

            var students = accountIds
                           .Select(accountId => {
                               var schoolName = Faker.Company.Name();
                               if (schoolName.Length > 50) schoolName = schoolName[..50];
                               
                               var faculty = Faker.Company.BS();
                               if (faculty.Length > 50) faculty = faculty[..50];

                               var website = Faker.Internet.SecureUrl();
                               if (website.Length > 100) website = website[..100];
                               
                               return new Student {
                                   AccountId = accountId,
                                   SchoolName = schoolName,
                                   Faculty = faculty,
                                   PersonalUrl = website
                               };
                           })
                           .ToArray();

            var studentIds = await _generatorService.InsertMultipleStudents(students);
            if (studentIds is null) return null;
            
            var teachers = accountIds
                          .Select(accountId => {
                              var company = Faker.Company.Name();
                              if (company.Length > 50) company = company[..50];
                               
                              var title = Faker.Company.BS();
                              if (title.Length > 50) title = title[..50];

                              var website = Faker.Internet.SecureUrl();
                              if (website.Length > 100) website = website[..100];
                              
                              return new Teacher {
                                  AccountId = accountId,
                                  Company = company,
                                  JobTitle = title,
                                  PersonalWebsite = website
                              };
                          })
                          .ToArray();

            var teacherIds = await _generatorService.InsertMultipleTeachers(teachers);
            return typeof(T).Name.Equals(nameof(Teacher)) ? teacherIds : studentIds;
        }

        private static Account GenerateAccount() {
            var firstName = Faker.Name.First();
            var lastName = Faker.Name.Last();
            
            var emailAddress = $"{ firstName.ToLower() }.{ lastName.ToLower() }@{ EmailDomains[Helpers.GetRandomNumberInRangeInclusive(EmailDomains.Length - 1)] }";
            if (emailAddress.Length > 100) emailAddress = emailAddress[..100];

            var username = $"{ firstName.ToLower() }.{ lastName.ToLower() }";
            if (username.Length > 50) username = username[..50];

            var phoneNumber = Faker.Phone.Number();
            if (phoneNumber.Length > 20) phoneNumber = phoneNumber[..20];
            
            var fullname = $"{ firstName } { lastName }";
            if (fullname.Length > 100) fullname = fullname[..100];
                
            return new Account {
                EmailAddress = emailAddress,
                EmailConfirmed = true,
                Username = username,
                NormalizedUsername = username.ToUpper(),
                PhoneNumber = phoneNumber,
                PreferredName = fullname
            };
        }

        private static Classroom GenerateClassroom(string teacherId) {
            var className = string.Join(SharedConstants.MonoSpace, Faker.Lorem.Words(Helpers.GetRandomNumberInRangeInclusive(3, 2)));
            if (className.Length > 70) className = className[..70];

            className = className.First().ToString().ToUpper() + className[1..];
            var capacities = new short[] { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500 }; // new short[] { 10,20,30,40,50 };

            return new Classroom {
                TeacherId = teacherId,
                ClassName = className,
                Capacity = capacities[Helpers.GetRandomNumberInRangeInclusive(capacities.Length - 1)],
                Price = (decimal) (Helpers.GetRandomNumberInRangeInclusive(MaxPrice, MinPrice) * 1.0/100),
                CommencedOn = Helpers.GetRandomDateTime(DateTime.UtcNow.AddDays(60), DateTime.UtcNow.AddDays(90)),
                Duration = (byte) Helpers.GetRandomNumberInRangeInclusive(10, 1),
                DurationUnit = (byte) SharedEnums.DurationUnit.Weeks,
                IsActive = true,
                CreatedOn = Helpers.GetRandomDateTime(DateTime.UtcNow, DateTime.UtcNow.AddDays(29))
            };
        }

        private static MarkBreakdownVM[] GenerateEnrolmentMarks() {
            var numberOfTasks = Helpers.GetRandomNumberInRangeInclusive(5);
            if (numberOfTasks == 0) return default;

            var marks = new List<MarkBreakdownVM>();
            for (var i = 0; i < numberOfTasks; i++) {
                var total = Helpers.GetRandomNumberInRangeInclusive(200, 10);
                var rewarded = Helpers.GetRandomNumberInRangeInclusive(total, total/5);
                
                marks.Add(new MarkBreakdownVM {
                    TaskName = string.Join(SharedConstants.MonoSpace, Faker.Lorem.Words(Helpers.GetRandomNumberInRangeInclusive(4, 2))),
                    TotalMarks = total,
                    RewardedMarks = rewarded,
                    Comment = Faker.Lorem.Sentence(),
                    MarkedOn = Helpers.GetRandomDateTime(DateTime.UtcNow.AddDays(-365), DateTime.UtcNow)
                });
            }

            return marks.ToArray();
        }

        private static int GetRandomNumberOfEnrolmentByPriceRange(decimal price, short capacity) {
            Dictionary<KeyValuePair<decimal, decimal>, KeyValuePair<int, int>> enrolmentRanges = new() {
                { new KeyValuePair<decimal, decimal>(0, 50), new KeyValuePair<int, int>(capacity/4, capacity) },
                { new KeyValuePair<decimal, decimal>(50, 100), new KeyValuePair<int, int>(capacity/10, capacity*9/10) },
                { new KeyValuePair<decimal, decimal>(100, 250), new KeyValuePair<int, int>(capacity/15, capacity*4/5) },
                { new KeyValuePair<decimal, decimal>(250, 500), new KeyValuePair<int, int>(capacity/15, capacity*3/5) },
                { new KeyValuePair<decimal, decimal>(500, 1000), new KeyValuePair<int, int>(capacity/15, capacity*7/15) },
                { new KeyValuePair<decimal, decimal>(1000, 10000), new KeyValuePair<int, int>(capacity/15, capacity/3) }
            };

            var (min, max) = enrolmentRanges
                             .Where(pair => pair.Key.Key <= price && pair.Key.Value >= price)
                             .Select(pair => pair.Value)
                             .First();

            return Helpers.GetRandomNumberInRangeInclusive(max, min);
        }
    }
}