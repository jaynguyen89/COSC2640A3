using System.Threading.Tasks;

namespace AssistantLibrary.Interfaces {

    public interface ISmsService {

        Task<bool?> SendSmsWithContent(string content);
    }
}