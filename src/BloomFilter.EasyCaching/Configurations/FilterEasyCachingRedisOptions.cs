using BloomFilter.Configurations;
using EasyCaching.Core;

namespace BloomFilter.CSRedis.Configurations
{
    public class FilterEasyCachingRedisOptions : FilterMemoryOptions
    {
        /// <summary>
        /// The Name
        /// </summary>
        public new string Name { get; set; } = BloomFilterConstValue.DefaultRedisName;

        /// <summary>
        /// The Redis Key
        /// </summary>
        public string RedisKey { get; set; } = "BloomFilter.Core";

        /// <summary>
        /// The <see cref="IRedisCachingProvider"/> Name
        /// </summary>
        public string? ProviderName { get; set; }
    }
}