using System.Collections.Generic;
using Helper;

namespace COSC2640A3.Models {

    public partial class Classroom {

        public string[] VerifyClassroomData() {
            var errors = new List<string>();

            if (!Helpers.IsProperString(ClassName)) errors.Add($"{ nameof(ClassName) } is required.");
            if (Capacity < 0) errors.Add($"Class { nameof(Capacity) } should not be negative.");
            if ((double) Price > 9999.99 || Price < 0) errors.Add($"{ nameof(Price) } should be between 0 and 999,99.");
            if (Duration > 255) errors.Add($"{ nameof(Duration) } should be between 1 to 255.");

            return errors.ToArray();
        }
    }
}