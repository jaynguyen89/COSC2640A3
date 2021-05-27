using System.Threading.Tasks;
using AssistantLibrary.Models;
using static Helper.Shared.SharedEnums;

namespace AssistantLibrary.Interfaces {

    public interface IDictionaryService {

        Task<Translation> GetSynonymsOrAntonymsFor(string word, bool synonym = true);
        
        Task<Translation> Translate(Language target, string word);
    }
}