using System;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

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
    }
}