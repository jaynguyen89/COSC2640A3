using System;
using System.Linq;
using System.Threading.Tasks;
using COSC2640A3.DbContexts;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class InvoiceService : IInvoiceService {
        
        private readonly ILogger<InvoiceService> _logger;
        private readonly MainDbContext _dbContext;

        public InvoiceService(
            ILogger<InvoiceService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Invoice> GetInvoiceByEnrolmentId(string enrolmentId) {
            try {
                return await _dbContext.Enrolments
                                       .Where(enrolment => enrolment.Id.Equals(enrolmentId))
                                       .Select(enrolment => enrolment.Invoice)
                                       .SingleOrDefaultAsync();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(InvoiceService) }.{ nameof(GetInvoiceByEnrolmentId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<bool?> UpdateInvoice(Invoice invoice) {
            _dbContext.Invoices.Update(invoice);
            
            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(InvoiceService) }.{ nameof(UpdateInvoice) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }
    }
}