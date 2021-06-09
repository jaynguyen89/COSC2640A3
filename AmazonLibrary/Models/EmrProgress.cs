using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace AmazonLibrary.Models {

    public sealed class EmrProgress {
        
        [DynamoDBHashKey]
        public string Id { get; set; }
        
        public long Timestamp { get; set; }
        
        public bool MapperDone { get; set; }
        
        public bool ReducerDone { get; set; }

        public static implicit operator EmrProgress(Dictionary<string, AttributeValue> item) {
            return new() {
                Id = item[nameof(Id)].S,
                Timestamp = long.Parse(item[nameof(Timestamp)].N),
                MapperDone = item[nameof(MapperDone)].BOOL,
                ReducerDone = item[nameof(ReducerDone)].BOOL
            };
        }
    }

    public sealed class EmrStatistics {
        
        [DynamoDBHashKey]
        public string Id { get; set; }
        
        public string Statistics { get; set; }
        
        public long Timestamp { get; set; }

        public static implicit operator EmrStatistics(Dictionary<string, AttributeValue> item) {
            return new() {
                Id = item[nameof(Id)].S,
                Timestamp = long.Parse(item[nameof(Timestamp)].N),
                Statistics = item[nameof(Statistics)].S
            };
        }
    }
}