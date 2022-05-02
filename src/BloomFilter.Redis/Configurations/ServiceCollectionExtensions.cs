using BloomFilter;
using BloomFilter.Configurations;
using BloomFilter.Redis.Configurations;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Uses the redis.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="name"></param>
        /// <param name="setupActions"></param>
        public static BloomFilterOptions UseRedis(this BloomFilterOptions options,
            string name = BloomFilterConstValue.DefaultRedisName, Action<FilterRedisOptions>? setupActions = null)
        {
            var filterRedisOptions = new FilterRedisOptions
            {
                Name = name
            };
            setupActions?.Invoke(filterRedisOptions);
            options.RegisterExtension(new FilterRedisOptionsExtension(filterRedisOptions));
            return options;
        }

        /// <summary>
        /// Uses the redis.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="filterRedisOptions"></param>
        public static BloomFilterOptions UseRedis(this BloomFilterOptions options, FilterRedisOptions filterRedisOptions)
        {
            if (filterRedisOptions == null) throw new ArgumentNullException(nameof(filterRedisOptions));
            options.RegisterExtension(new FilterRedisOptionsExtension(filterRedisOptions));
            return options;
        }
    }
}