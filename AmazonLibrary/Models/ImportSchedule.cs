using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace AmazonLibrary.Models {

    public sealed class ImportSchedule {
        
        [DynamoDBHashKey]
        public string Id { get; set; }
        
        public string AccountId { get; set; }
        
        public string FileId { get; set; }
        
        public string FileName { get; set; }
        
        public long FileSize { get; set; }
        
        public long UploadedOn { get; set; }
        
        public byte Status { get; set; }
        
        public bool IsForClassroom { get; set; }

        public Document CreateItemDocument() {
            return new() {
                [nameof(Id)] = Guid.NewGuid().ToString(),
                [nameof(AccountId)] = AccountId,
                [nameof(FileId)] = FileId,
                [nameof(FileName)] = FileName,
                [nameof(FileSize)] = FileSize,
                [nameof(UploadedOn)] = UploadedOn,
                [nameof(Status)] = Status
            };
        }

        public static implicit operator ImportSchedule(Dictionary<string, AttributeValue> item) {
            return new() {
                Id = item[nameof(Id)].S,
                AccountId = item[nameof(AccountId)].S,
                FileId = item[nameof(FileId)].S,
                FileName = item[nameof(FileName)].S,
                FileSize = long.Parse(item[nameof(FileSize)].N),
                UploadedOn = long.Parse(item[nameof(UploadedOn)].N),
                Status = byte.Parse(item[nameof(Status)].N)
            };
        }
    }
}