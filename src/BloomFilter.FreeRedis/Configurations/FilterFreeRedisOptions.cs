using BloomFilter.Configurations;
using FreeRedis;
using System;
using System.Collections.Generic;

namespace BloomFilter.FreeRedis.Configurations
{
    public class FilterFreeRedisOptions : FilterMemoryOptions
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
        public RedisClient? Client { get; set; }
    }
}