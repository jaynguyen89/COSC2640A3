namespace AmazonLibrary {

    public sealed class AmazonOptions {
        
        public string AwsAccessKeyId { get; set; }
        
        public string AwsSecretKey { get; set; }
        
        public string RegionEndpoint { get; set; }
        
        public string S3TimeoutSeconds { get; set; }
    }
}