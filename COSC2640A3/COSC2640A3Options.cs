namespace COSC2640A3 {

    public sealed class COSC2640A3Options {

        public string DevelopmentConnectionString { get; set; }
        
        public string ProductionConnectionString { get; set; }
        
        public string DevelopmentCacheEndpoint { get; set; }
        
        public string ProductionCacheEndpoint { get; set; }
        
        public string CacheStoreName { get; set; }
        
        public string CacheSlidingExpiration { get; set; }
        
        public string CacheAbsoluteExpiration { get; set; }
        
        public string CacheEnabled { get; set; }
    }
}
