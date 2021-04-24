using System.Threading.Tasks;

namespace COSC2640A3.Services.Interfaces {

    public interface IRoleService {

        /// <summary>
        /// Returns null if error, true if success, false if failed.
        /// </summary>
        Task<bool?> CreateRolesForAccountById(string accountId);
    }
}