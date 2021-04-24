using COSC2640A3.Models;

namespace COSC2640A3.ViewModels.Account {

    public class StudentVM : AccountVM {
        
        public string StudentId { get; set; }
        
        public string SchoolName { get; set; }
        
        public string Faculty { get; set; }
        
        public string PersonalUrl { get; set; }

        public static implicit operator StudentVM(Student student) {
            return new() {
                Email = student.Account.EmailAddress,
                Username = student.Account.Username,
                PhoneNumber = student.Account.PhoneNumber,
                PhoneNumberConfirmed = student.Account.PhoneNumberConfirmed,
                TwoFaEnabled = student.Account.TwoFactorEnabled,
                PreferredName = student.Account.PreferredName,
                StudentId = student.Id,
                SchoolName = student.SchoolName,
                Faculty = student.Faculty,
                PersonalUrl = student.PersonalUrl
            };
        }
    }
}