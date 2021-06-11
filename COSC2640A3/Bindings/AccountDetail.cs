using System.Collections.Generic;
using COSC2640A3.Models;
using Helper;

namespace COSC2640A3.Bindings {

    public class AccountDetail {
        
        public string Id { get; set; } // Student or Teacher ID, not Account ID
    }

    public sealed class StudentDetail : AccountDetail {

        public string SchoolName { get; set; }
        
        public string Faculty { get; set; }
        
        public string PersonalUrl { get; set; }
        
        public string[] VerifyDetail(bool isStudent = true) {
            var errors = new List<string>();

            if (!Helpers.IsProperString(SchoolName)) SchoolName = null;
            if (!Helpers.IsProperString(Faculty)) Faculty = null;
            if (!Helpers.IsProperString(PersonalUrl)) PersonalUrl = null;
            
            if (SchoolName?.Length > 50) errors.Add($"{ (isStudent ? nameof(Student.SchoolName).ToHumanStyled() : nameof(Teacher.Company)) } is too long. Max 50 characters.");
            if (Faculty?.Length > 50) errors.Add($"{ (isStudent ? nameof(Student.Faculty) : nameof(Teacher.JobTitle).ToHumanStyled()) } is too long. Max 50 characters.");
            if (PersonalUrl?.Length > 100) errors.Add("Personal URL is too long. Max 100 characters.");

            return errors.ToArray();
        }
    }

    public sealed class TeacherDetail : AccountDetail {

        public string Company { get; set; }
        
        public string JobTitle { get; set; }
        
        public string PersonalWebsite { get; set; }

        public string[] VerifyDetail(bool isStudent = true) {
            var errors = new List<string>();

            if (!Helpers.IsProperString(Company)) Company = null;
            if (!Helpers.IsProperString(JobTitle)) JobTitle = null;
            if (!Helpers.IsProperString(PersonalWebsite)) PersonalWebsite = null;
            
            if (Company?.Length > 50) errors.Add($"{ (isStudent ? nameof(Student.SchoolName).ToHumanStyled() : nameof(Teacher.Company)) } is too long. Max 50 characters.");
            if (JobTitle?.Length > 50) errors.Add($"{ (isStudent ? nameof(Student.Faculty) : nameof(Teacher.JobTitle).ToHumanStyled()) } is too long. Max 50 characters.");
            if (PersonalWebsite?.Length > 100) errors.Add("Personal URL is too long. Max 100 characters.");

            return errors.ToArray();
        }
    }
}