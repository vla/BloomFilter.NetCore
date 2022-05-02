using BloomFilter;
using BloomFilter.Configurations;
using BloomFilter.CSRedis.Configurations;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Uses the EasyCaching redis.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="name"></param>
        /// <param name="setupActions"></param>
        public static BloomFilterOptions UseEasyCachingRedis(this BloomFilterOptions options,
            string name = BloomFilterConstValue.DefaultRedisName, Action<FilterEasyCachingRedisOptions>? setupActions = null)
        {
            var filterRedisOptions = new FilterEasyCachingRedisOptions
            {
                Name = name
            };
            setupActions?.Invoke(filterRedisOptions);
            options.RegisterExtension(new FilterEasyCachingRedisExtension(filterRedisOptions));
            return options;
        }

        /// <summary>
        /// Uses the EasyCaching redis.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="filterRedisOptions"></param>
        public static BloomFilterOptions UseEasyCachingRedis(this BloomFilterOptions options, FilterEasyCachingRedisOptions filterRedisOptions)
        {
            if (filterRedisOptions == null) throw new ArgumentNullException(nameof(filterRedisOptions));
            options.RegisterExtension(new FilterEasyCachingRedisExtension(filterRedisOptions));
            return options;
        }
    }
}