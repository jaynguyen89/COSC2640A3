namespace COSC2640A3 {

    public sealed class MainOptions {

        // Database connection strings
        public string DevelopmentConnectionString { get; set; }
        
        public string ProductionConnectionString { get; set; }
        
        // For RedisCache setup
        public string DevelopmentCacheEndpoint { get; set; }
        
        public string ProductionCacheEndpoint { get; set; }
        
        public string CacheStoreName { get; set; }
        
        public string CacheSlidingExpiration { get; set; }
        
        public string CacheAbsoluteExpiration { get; set; }
        
        public string CacheEnabled { get; set; }
        
        // For AWS Cognito service
        public string AwsRegion { get; set; }
        
        public string CognitoUserPoolId { get; set; }
        
        public string UserPoolAppClientId { get; set; }
        
        public string PoolIdentityProviderUrl { get; set; }
    }
}
