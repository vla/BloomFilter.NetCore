using BloomFilter.Configurations;
using StackExchange.Redis;
using System.Collections.Generic;

namespace BloomFilter.Redis.Configurations
{
    public class FilterRedisOptions : FilterMemoryOptions
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
        /// Gets or sets the Redis database index the cache will use.
        /// </summary>
        public int Database { get; set; } = 0;

        /// <summary>
        /// Gets or sets the username to be used to connect to the Redis server.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password to be used to connect to the Redis server.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SSL encryption.
        /// </summary>
        public bool IsSsl { get; set; } = false;

        /// <summary>
        /// Gets or sets the SSL Host.
        /// If set, it will enforce this particular host on the server's certificate.
        /// </summary>
        public string? SslHost { get; set; }

        /// <summary>
        /// Gets or sets the timeout for any connect operations.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5000;

        /// <summary>
        /// Gets the list of endpoints to be used to connect to the Redis server.
        /// see: IP:Port
        /// </summary>
        public IList<string> Endpoints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether this FilterRedisOptions
        /// allow admin.
        /// </summary>
        public bool AllowAdmin { get; set; } = false;

        /// <summary>
        /// Gets or sets the string configuration.
        /// </summary>
        public string? Configuration { get; set; }

        /// <summary>
        /// Shared IConnectionMultiplexer
        /// </summary>
        public IConnectionMultiplexer? Connection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this FilterRedisOptions
        /// abort on connect fail.
        /// </summary>
        public bool AbortOnConnectFail { get; set; } = false;
    }
}