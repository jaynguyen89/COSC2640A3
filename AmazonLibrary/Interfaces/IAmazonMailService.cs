using System.Threading.Tasks;
using AmazonLibrary.Models;

namespace AmazonLibrary.Interfaces {

    public interface IAmazonMailService {

        Task<bool?> SendEmailSingle(EmailComposer emailComposer);
    }
}