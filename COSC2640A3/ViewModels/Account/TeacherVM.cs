using COSC2640A3.Models;

namespace COSC2640A3.ViewModels.Account {

    public sealed class TeacherVM : AccountVM {
        
        public string TeacherId { get; set; }
        
        public string Company { get; set; }
        
        public string JobTitle { get; set; }
        
        public string PersonalWebsite { get; set; }
        
        public static implicit operator TeacherVM(Teacher teacher) {
            return new() {
                Email = teacher.Account.EmailAddress,
                Username = teacher.Account.Username,
                PhoneNumber = teacher.Account.PhoneNumber,
                PhoneNumberConfirmed = teacher.Account.PhoneNumberConfirmed,
                TwoFaEnabled = teacher.Account.TwoFactorEnabled,
                PreferredName = teacher.Account.PreferredName,
                TeacherId = teacher.Id,
                Company = teacher.Company,
                JobTitle = teacher.JobTitle,
                PersonalWebsite = teacher.PersonalWebsite
            };
        }
    }
}