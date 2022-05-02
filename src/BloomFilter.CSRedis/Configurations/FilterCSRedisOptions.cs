using BloomFilter.Configurations;
using CSRedis;
using System;
using System.Collections.Generic;

namespace BloomFilter.CSRedis.Configurations
{
    public class FilterCSRedisOptions : FilterMemoryOptions
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
        /// Gets or sets the node rule.
        /// </summary>
        public Func<string, string>? NodeRule { get; set; }

        /// <summary>
        /// Gets or sets the connection strings.
        /// </summary>
        public List<string>? ConnectionStrings { get; set; }

        /// <summary>
        /// Gets or sets the sentinels settings.
        /// </summary>
        public List<string>? Sentinels { get; set; }

        /// <summary>
        /// Gets or sets the read write setting for sentinel mode.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Shared CSRedisClient
        /// </summary>
        public CSRedisClient? Client { get; set; }
    }
}