using COSC2640A3.Bindings;

namespace COSC2640A3.Models {

    public partial class Teacher {

        public void UpdateDetail(TeacherDetail detail) {
            Company = detail.Company;
            JobTitle = detail.JobTitle;
            PersonalWebsite = detail.PersonalWebsite;
        }
    }
}