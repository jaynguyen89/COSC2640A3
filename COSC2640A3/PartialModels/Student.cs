using COSC2640A3.Bindings;

namespace COSC2640A3.Models {

    public partial class Student {

        public void UpdateDetail(AccountDetail detail) {
            SchoolName = detail.Workplace;
            Faculty = detail.Department;
            PersonalUrl = detail.PersonalUrl;
        }
    }
}