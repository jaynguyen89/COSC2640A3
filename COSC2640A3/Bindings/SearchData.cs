using System.Collections.Generic;
using System.Text.RegularExpressions;
using Helper;
using Helper.Shared;

namespace COSC2640A3.Bindings {

    public sealed class SearchData {
        
        public string ClassroomName { get; set; }
        
        public string TeacherName { get; set; }

        public string[] VerifySearchData() {
            var errors = new List<string>();
            
            if (!Helpers.IsProperString(ClassroomName) && !Helpers.IsProperString(TeacherName))
                errors.Add($"{ nameof(ClassroomName).ToHumanStyled() } and { nameof(TeacherName).ToHumanStyled() } should not be both empty.");

            ClassroomName = Regex.Replace(ClassroomName?.Trim() ?? string.Empty, SharedConstants.MultiSpace, SharedConstants.MonoSpace);
            TeacherName = Regex.Replace(TeacherName?.Trim() ?? string.Empty, SharedConstants.MultiSpace, SharedConstants.MonoSpace);

            return errors.ToArray();
        }
    }
}