using System.Threading.Tasks;

namespace AmazonLibrary.Interfaces {

    public interface ITextractService {

        Task<string> StartTextDetectionJobForFile(string fileId);
        
        Task<string[]> GetExtractedTextsFromTextractJob(string jobId);
    }
}