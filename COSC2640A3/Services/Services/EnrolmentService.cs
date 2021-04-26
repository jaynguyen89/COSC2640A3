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
    }
}