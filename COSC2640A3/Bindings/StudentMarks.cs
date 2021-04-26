using System;
using System.Linq;
using COSC2640A3.ViewModels.Features;

namespace COSC2640A3.Bindings {

    public sealed class StudentMarks {
        
        public string EnrolmentId { get; set; }
        
        public MarkBreakdownVM[] MarkBreakdowns { get; set; }

        public string[] VerifyStudentMarks() {
            if (MarkBreakdowns.Length == 0) return new[] { "No marks has been entered." };
            
            return MarkBreakdowns.Select((mark, i) => new { i, e = mark.VerifyMarks() })
                                 .Where(pair => pair.e.Length != 0)
                                 .Select(pair => $"Mark entry No.#{ (pair.i + 1) }: { pair.e.Aggregate((e1, e2) => $"{ e1 } { e2 }") }")
                                 .ToArray();
        }

        public int CalculateOverallMarks() {
            var totalSum = MarkBreakdowns.Select(mark => mark.TotalMarks).Aggregate((former, latter) => former + latter);
            var rewardSum = MarkBreakdowns.Select(mark => mark.RewardedMarks).Aggregate((former, latter) => former + latter);

            var average = rewardSum * 100.0 / totalSum;
            return (int) (average % 1 >= 0.5 ? Math.Ceiling(average) : Math.Floor(average));
        }
    }
}