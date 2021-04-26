using System;
using System.Linq;
using System.Threading.Tasks;
using COSC2640A3.DbContexts;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class ClassroomService : IClassroomService {
        
        private readonly ILogger<ClassroomService> _logger;
        private readonly MainDbContext _dbContext;

        public ClassroomService(
            ILogger<ClassroomService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<string> InsertNewClassroom(Classroom classroom) {
            await _dbContext.Classrooms.AddAsync(classroom);

            try {
                await _dbContext.SaveChangesAsync();
                return classroom.Id;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(ClassroomService) }.{ nameof(InsertNewClassroom) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> UpdateClassroom(Classroom classroom) {
            _dbContext.Classrooms.Update(classroom);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(ClassroomService) }.{ nameof(UpdateClassroom) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> IsClassroomBelongedToThisTeacherByAccountId(string accountId, string classroomId) {
            try {
                return await _dbContext.Teachers
                                       .Where(teacher => teacher.AccountId.Equals(accountId))
                                       .SelectMany(teacher => teacher.Classrooms)
                                       .AnyAsync(classroom => classroom.Id.Equals(classroomId));
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(ClassroomService) }.{ nameof(IsClassroomBelongedToThisTeacherByAccountId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<Classroom> GetClassroomById(string classroomId) {
            return await _dbContext.Classrooms.FindAsync(classroomId);
        }

        public async Task<bool?> DeleteClassroom(Classroom classroom) {
            _dbContext.Classrooms.Remove(classroom);
            
            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(ClassroomService) }.{ nameof(DeleteClassroom) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<ClassroomVM[]> GetAllClassroomsByTeacherId(string teacherId) {
            try {
                return await _dbContext.Classrooms
                                       .Where(classroom => classroom.TeacherId.Equals(teacherId))
                                       .Select(classroom => (ClassroomVM) classroom)
                                       .ToArrayAsync();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(ClassroomService) }.{ nameof(GetAllClassroomsByTeacherId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<ClassroomVM[]> GetAllClassrooms() {
            try {
                return await _dbContext.Classrooms
                                       .Select(classroom => (ClassroomVM) classroom)
                                       .ToArrayAsync();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(ClassroomService) }.{ nameof(GetAllClassrooms) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<ClassroomVM> GetClassroomDetailsFor(string classroomId, bool forStudent = true) {
            _logger.LogInformation($"{ nameof(ClassroomService) }.{ nameof(GetClassroomDetailsFor) }: { nameof(classroomId) }={ classroomId }, { nameof(forStudent) }={ forStudent }.");

            var classroom = await _dbContext.Classrooms.FindAsync(classroomId);
            var classroomDetail = (ClassroomVM) classroom;
            classroomDetail.SetClassroomDetail(classroom);
                
            if (forStudent) return classroomDetail;
            
            //TODO: set additional classroom contents like files, audio, videos... attached by the teacher
            return classroomDetail;
        }

        public async Task<EnrolmentVM[]> GetStudentEnrolmentsByClassroomId(string classroomId) {
            try {
                var enrolmentData = await _dbContext.Enrolments
                                                    .Where(enrolment => enrolment.ClassroomId.Equals(classroomId))
                                                    .Select(enrolment => new { envolmentVm = (EnrolmentVM) enrolment, invoice = enrolment.Invoice })
                                                    .ToArrayAsync();

                return enrolmentData
                       .Select(pair => {
                           var enrolment = pair.envolmentVm;
                           if (pair.invoice.IsPaid) enrolment.Invoice.PaymentDetail = pair.invoice;
                           return enrolment;
                       })
                       .ToArray();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(GetStudentEnrolmentsByClassroomId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<Enrolment[]> GetEnrolmentsByClassroomId(string classroomId) {
            try {
                return await _dbContext.Enrolments
                                       .Where(enrolment => enrolment.ClassroomId.Equals(classroomId))
                                       .ToArrayAsync();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(GetEnrolmentsByClassroomId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }
    }
}