namespace AmazonLibrary {

    public sealed class AmazonOptions {
        
        // For AWS connection setup
        public string AwsAccessKeyId { get; set; }
        
        public string AwsSecretKey { get; set; }
        
        public string RegionEndpoint { get; set; }
        
        public string S3TimeoutSeconds { get; set; }
        
        // For AWS S3
        public string S3BucketTeacherImportingClassrooms { get; set; }
        
        public string S3BucketTeacherImportingStudentsToClassroom { get; set; }
        
        // For AWS DynamoDb
        public string ClassroomImportSchedulesTableName { get; set; }
        
        public string StudentImportSchedulesTableName { get; set; }
        
        // For AWS Simple Mail Service
        public string MailSentFromAddress { get; set; }
        
        // For EMR service
        public string EmrMasterNodeEndpointUrl { get; set; }
        
        public string EmrMasterNodeEc2Username { get; set; }
        
        public string KeyFingerprint { get; set; }
        
        public string EmrProgressTableName { get; set; }
        
        public string EmrResultsTableName { get; set; }
    }
}