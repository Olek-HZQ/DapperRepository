
namespace DapperRepo.Core.Configuration
{
    public class AppConfig
    {
        public bool RedisEnabled { get; set; }

        /// <summary>
        /// Gets or sets Redis connection string. Used when Redis caching is enabled
        /// </summary>
        public string RedisCachingConnectionString { get; set; }

        public string CurrentDbTypeName { get; set; }
    }
}
