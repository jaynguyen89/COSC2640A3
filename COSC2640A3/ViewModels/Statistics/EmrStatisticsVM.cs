using System;

namespace COSC2640A3.ViewModels {

    public sealed class EmrStatisticsVM {
        
        public string Id { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public double Range50 { get; set; }
        
        public double Range100 { get; set; }
        
        public double Range250 { get; set; }
        
        public double Range500 { get; set; }
        
        public double Range1000 { get; set; }
        
        public double Range1001 { get; set; }
    }
}