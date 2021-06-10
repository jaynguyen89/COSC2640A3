using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class DataCache
    {
        public string Id { get; set; }
        public string DataType { get; set; }
        public string DataId { get; set; }
        public string DataKey { get; set; }
        public string SearchInput { get; set; }
        public string SerializedData { get; set; }
        public long Timestamp { get; set; }
    }
}
