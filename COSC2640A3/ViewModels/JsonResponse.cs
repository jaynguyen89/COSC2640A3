using Helper.Shared;

namespace COSC2640A3.ViewModels {

    public sealed class JsonResponse {

        public SharedEnums.RequestResult Result { get; set; } = SharedEnums.RequestResult.Success;
        
        public string[] Messages { get; set; }
        
        public object Data { get; set; }
    }
}