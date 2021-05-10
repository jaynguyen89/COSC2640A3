using System;
using System.Linq;
using System.Threading.Tasks;
using COSC2640A3.DbContexts;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels.Exports;
using COSC2640A3.ViewModels.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class EnrolmentService : IEnrolmentService {
        
        private readonly ILogger<EnrolmentService> _logger;
        private readonly MainDbContext _dbContext;

        public EnrolmentService(
            ILogger<EnrolmentService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<string> InsertNewEnrolment(Enrolment enrolment) {
            await _dbContext.Enrolments.AddAsync(enrolment);

            try {
                await _dbContext.SaveChangesAsync();
                return enrolment.Id;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(EnrolmentService) }.{ nameof(InsertNewEnrolment) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<string> InsertNewInvoice(Invoice invoice) {
            await _dbContext.Invoices.AddAsync(invoice);

            try {
                await _dbContext.SaveChangesAsync();
                return invoice.Id;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(EnrolmentService) }.{ nameof(InsertNewInvoice) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> IsEnrolmentMadeByStudentByAccountId(string enrolmentId, string accountId) {
            try {
                return await _dbContext.Enrolments.AnyAsync(enrolment => enrolment.Id.Equals(enrolmentId) && enrolment.Student.AccountId.Equals(accountId));
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(IsEnrolmentMadeByStudentByAccountId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<Enrolment> GetEnrolmentById(string enrolmentId) {
            return await _dbContext.Enrolments.FindAsync(enrolmentId);
        }

        public async Task<bool?> DeleteEnrolment(Enrolment enrolment) {
            _dbContext.Enrolments.Remove(enrolment);
            
            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(EnrolmentService) }.{ nameof(DeleteEnrolment) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<Invoice> GetInvoiceByEnrolmentId(string enrolmentId) {
            return await _dbContext.Invoices.FindAsync(enrolmentId);
        }

        public async Task<bool?> DeleteInvoice(Invoice invoice) {
            _dbContext.Invoices.Remove(invoice);
            
            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(EnrolmentService) }.{ nameof(DeleteInvoice) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<EnrolmentVM[]> GetStudentEnrolmentsByAccountId(string accountId) {
            try {
                var enrolmentData = await _dbContext.Enrolments
                                       .Where(enrolment => enrolment.Student.AccountId.Equals(accountId))
                                       .Select(enrolment => new {
                                           envolmentVm = (EnrolmentVM) enrolment,
                                           invoice = enrolment.Invoice,
                                           classroom = enrolment.Classroom,
                                           teacherName = enrolment.Classroom.Teacher.Account.PreferredName
                                       })
                                       .ToArrayAsync();

                return enrolmentData
                       .Select(enrolmentInfo => {
                           var enrolment = enrolmentInfo.envolmentVm;
                           enrolment.Classroom = enrolmentInfo.classroom;
                           enrolment.Classroom.TeacherName = enrolmentInfo.teacherName;
                           if (enrolmentInfo.invoice.IsPaid) enrolment.Invoice.PaymentDetail = enrolmentInfo.invoice;
                           return enrolment;
                       })
                       .ToArray();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(GetStudentEnrolmentsByAccountId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<bool?> UpdateEnrolment(Enrolment enrolment) {
            _dbContext.Enrolments.Update(enrolment);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(EnrolmentService) }.{ nameof(UpdateEnrolment) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> DoesEnrolmentRelateToAClassroomOfThisTeacher(string enrolmentId, string teacherId) {
            try {
                return await _dbContext.Enrolments
                                       .Where(enrolment => enrolment.Id.Equals(enrolmentId))
                                       .Select(enrolment => enrolment.Classroom)
                                       .AnyAsync(classroom => classroom.TeacherId.Equals(teacherId));
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(DoesEnrolmentRelateToAClassroomOfThisTeacher) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<bool?> UpdateMultipleEnrolments(Enrolment[] enrolments) {
            _dbContext.Enrolments.UpdateRange(enrolments);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(EnrolmentService) }.{ nameof(UpdateMultipleEnrolments) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public EnrolmentExportVM[] GetEnrolmentDataForExportBy(string[] classroomIds) {
            try {
                var queryableEnrolments = _dbContext.Enrolments
                                                    .Where(enrolment => classroomIds.Contains(enrolment.ClassroomId))
                                                    .Select(enrolment => new { ClassroomId = enrolment.ClassroomId, Enrolment = enrolment, Invoice = enrolment.Invoice })
                                                    .AsEnumerable();
                
                return queryableEnrolments
                       .GroupBy(enrolment => enrolment.ClassroomId)
                       .Select(group => new EnrolmentExportVM {
                           ClassroomId = group.Key, 
                           Enrolments = group.Select(item => {
                               var enrolmentVm = (EnrolmentExportVM.EnrolmentExport) item.Enrolment;
                               enrolmentVm.Invoice = item.Invoice;
                               return enrolmentVm;

                           }).ToArray()
                       }).ToArray();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(GetEnrolmentDataForExportBy) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }
    }
}