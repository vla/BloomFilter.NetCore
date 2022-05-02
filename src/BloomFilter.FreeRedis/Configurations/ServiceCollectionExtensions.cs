using BloomFilter;
using BloomFilter.Configurations;
using BloomFilter.FreeRedis.Configurations;
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
        public static BloomFilterOptions UseFreeRedis(this BloomFilterOptions options,
            string name = BloomFilterConstValue.DefaultRedisName, Action<FilterFreeRedisOptions>? setupActions = null)
        {
            var filterRedisOptions = new FilterFreeRedisOptions
            {
                Name = name
            };
            setupActions?.Invoke(filterRedisOptions);
            options.RegisterExtension(new FilterFreeRedisOptionsExtension(filterRedisOptions));
            return options;
        }

        /// <summary>
        /// Uses the redis.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="filterRedisOptions"></param>
        public static BloomFilterOptions UseFreeRedis(this BloomFilterOptions options, FilterFreeRedisOptions filterRedisOptions)
        {
            if (filterRedisOptions == null) throw new ArgumentNullException(nameof(filterRedisOptions));
            options.RegisterExtension(new FilterFreeRedisOptionsExtension(filterRedisOptions));
            return options;
        }
    }
}