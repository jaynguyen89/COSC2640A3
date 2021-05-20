using System.Threading.Tasks;
using COSC2640A3.Models;

namespace COSC2640A3.Services.Interfaces {

    public interface IInvoiceService {

        Task<Invoice> GetInvoiceByEnrolmentId(string enrolmentId);
        
        Task<bool?> UpdateInvoice(Invoice invoice);
    }
}