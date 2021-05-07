using System.Threading.Tasks;
using AmazonLibrary.Models;
using Helper.Shared;

namespace AmazonLibrary.Interfaces {

    public interface IDynamoService {

        Task<string> SaveToSchedulesTable(ImportSchedule schedule, SharedEnums.ImportType importType);
    }
}