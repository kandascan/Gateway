namespace Gateway.Config
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = "localhost:6379";
        public int DefaultCacheExpiration { get; set; } = 30;
    }

}
