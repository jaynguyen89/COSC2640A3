using System;
using AmazonLibrary.Models;

namespace COSC2640A3.ViewModels {

    public sealed class EmrProgressVM {
        
        public string Id { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public bool MapperDone { get; set; }
        
        public bool ReducerDone { get; set; }

        public static implicit operator EmrProgressVM(EmrProgress progress) {
            return new() {
                Id = progress.Id,
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(progress.Timestamp).UtcDateTime,
                MapperDone = progress.MapperDone,
                ReducerDone = progress.ReducerDone
            };
        }
    }
}