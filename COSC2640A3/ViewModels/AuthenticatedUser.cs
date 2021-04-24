using Helper.Shared;

namespace COSC2640A3.ViewModels {

    public sealed class AuthenticatedUser {
        
        public string AuthToken { get; set; }
        
        public string AccountId { get; set; }
        
        public SharedEnums.Role Role { get; set; }
    }
}