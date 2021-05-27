using COSC2640A3.Bindings;

namespace COSC2640A3.Models {

    public partial class Teacher {

        public void UpdateDetail(AccountDetail detail) {
            Company = detail.Workplace;
            JobTitle = detail.Department;
            PersonalWebsite = detail.PersonalUrl;
        }
    }
}