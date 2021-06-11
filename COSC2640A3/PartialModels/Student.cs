using COSC2640A3.Bindings;

namespace COSC2640A3.Models {

    public partial class Student {

        public void UpdateDetail(StudentDetail detail) {
            SchoolName = detail.SchoolName;
            Faculty = detail.Faculty;
            PersonalUrl = detail.PersonalUrl;
        }
    }
}