using System.Collections.Generic;
using System.Threading.Tasks;
using static Helper.Shared.SharedEnums;

namespace AmazonLibrary.Interfaces {

    public interface ITranslateService {

        Task<KeyValuePair<bool, string>> Translate(Language target, string phrase);
    }
}