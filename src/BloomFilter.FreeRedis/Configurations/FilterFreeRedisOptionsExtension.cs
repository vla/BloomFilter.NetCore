using BloomFilter.Configurations;
using FreeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace BloomFilter.FreeRedis.Configurations
{
    public class FilterFreeRedisOptionsExtension : IBloomFilterOptionsExtension
    {
        private readonly FilterFreeRedisOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterFreeRedisOptionsExtension"/> class.
        /// </summary>
        /// <param name="options">Configure.</param>
        public FilterFreeRedisOptionsExtension(FilterFreeRedisOptions options)
        {
            _options = options;
        }

        public void AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<IBloomFilterFactory, DefaultBloomFilterFactory>();

            services.AddSingleton<IBloomFilter, FilterFreeRedis>(x =>
            {
                FilterFreeRedis createFilter(RedisClient client)
                {
                    return new FilterFreeRedis(
                        _options.Name,
                        client,
                        _options.RedisKey,
                        _options.ExpectedElements,
                        _options.ErrorRate,
                        HashFunction.Functions[_options.Method]);
                }

                if (_options.Client != null)
                {
                    return createFilter(_options.Client);
                }

                if (_options.ConnectionStrings == null || _options.ConnectionStrings.Count == 0)
                {
                    throw new ArgumentException($"{nameof(FilterFreeRedisOptions.ConnectionStrings)} is Empty!");
                }

                if (_options.Sentinels != null && _options.Sentinels.Any())
                {
                    var redisClient = new RedisClient(_options.ConnectionStrings[0], _options.Sentinels.ToArray(), _options.ReadOnly);
                    return createFilter(redisClient);
                }

                if (_options.ConnectionStrings.Count == 1)
                {
                    var redisClient = new RedisClient(_options.ConnectionStrings[0]);
                    return createFilter(redisClient);
                }
                else
                {
                    var redisClient = new RedisClient(_options.ConnectionStrings.Select(a => (ConnectionStringBuilder)a).ToArray());
                    return createFilter(redisClient);
                }
            });
        }
    }
}