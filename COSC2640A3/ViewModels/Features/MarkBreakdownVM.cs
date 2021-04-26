using System;
using System.Collections.Generic;
using Helper;

namespace COSC2640A3.ViewModels.Features {

    public sealed class MarkBreakdownVM {
        
        public string TaskName { get; set; }
        
        public int TotalMarks { get; set; }
        
        public int RewardedMarks { get; set; }
        
        public DateTime MarkedOn { get; set; }
        
        public string Comment { get; set; }

        public string[] VerifyMarks() {
            var errors = new List<string>();
            
            if (!Helpers.IsProperString(TaskName)) errors.Add($"{ nameof(TaskName) } is missing.");
            if (TotalMarks == 0) errors.Add($"{ nameof(TotalMarks) } should be greater than 0.");
            if (RewardedMarks == 0) errors.Add($"{ nameof(RewardedMarks) } should be greater than 0.");
            if (TotalMarks < RewardedMarks) errors.Add($"{ nameof(TotalMarks) } should be greater than { nameof(RewardedMarks) }.");
            if (!Helpers.IsProperString(Comment)) Comment = null;

            return errors.ToArray();
        }
    }
}