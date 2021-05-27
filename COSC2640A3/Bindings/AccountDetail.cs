using System.Collections.Generic;
using COSC2640A3.Models;
using Helper;

namespace COSC2640A3.Bindings {

    public sealed class AccountDetail {
        
        public string Id { get; set; } // Student or Teacher ID, not Account ID
        
        public string Workplace { get; set; }
        
        public string Department { get; set; }
        
        public string PersonalUrl { get; set; }

        public string[] VerifyDetail(bool isStudent = true) {
            var errors = new List<string>();

            if (!Helpers.IsProperString(Workplace)) Workplace = null;
            if (!Helpers.IsProperString(Department)) Department = null;
            if (!Helpers.IsProperString(PersonalUrl)) PersonalUrl = null;
            
            if (Workplace?.Length > 50) errors.Add($"{ (isStudent ? nameof(Student.SchoolName).ToHumanStyled() : nameof(Teacher.Company)) } is too long. Max 50 characters.");
            if (Department?.Length > 50) errors.Add($"{ (isStudent ? nameof(Student.Faculty) : nameof(Teacher.JobTitle).ToHumanStyled()) } is too long. Max 50 characters.");
            if (PersonalUrl?.Length > 100) errors.Add("Personal URL is too long. Max 100 characters.");

            return errors.ToArray();
        }
    }
}