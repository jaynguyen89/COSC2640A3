using System.Threading.Tasks;
using AmazonLibrary.Models;

namespace AmazonLibrary.Interfaces {

    public interface IDynamoService {

        Task<string> SaveToSchedulesTable(ImportSchedule schedule, string importType);
    }

}