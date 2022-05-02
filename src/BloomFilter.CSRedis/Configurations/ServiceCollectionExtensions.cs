using BloomFilter;
using BloomFilter.Configurations;
using BloomFilter.CSRedis.Configurations;
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
        public static BloomFilterOptions UseCSRedis(this BloomFilterOptions options,
            string name = BloomFilterConstValue.DefaultRedisName, Action<FilterCSRedisOptions>? setupActions = null)
        {
            var filterRedisOptions = new FilterCSRedisOptions
            {
                Name = name
            };
            setupActions?.Invoke(filterRedisOptions);
            options.RegisterExtension(new FilterCSRedisOptionsExtension(filterRedisOptions));
            return options;
        }

        /// <summary>
        /// Uses the redis.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="filterRedisOptions"></param>
        public static BloomFilterOptions UseCSRedis(this BloomFilterOptions options, FilterCSRedisOptions filterRedisOptions)
        {
            if (filterRedisOptions == null) throw new ArgumentNullException(nameof(filterRedisOptions));
            options.RegisterExtension(new FilterCSRedisOptionsExtension(filterRedisOptions));
            return options;
        }
    }
}