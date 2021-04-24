using System.Threading.Tasks;

namespace COSC2640A3.Services.Interfaces {

    public interface IContextService {

        Task StartTransaction();

        Task ConfirmTransaction();

        Task RevertTransaction();
    }
}