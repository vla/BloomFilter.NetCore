using BloomFilter.Configurations;
using CSRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System;

namespace BloomFilter.CSRedis.Configurations
{
    public class FilterCSRedisOptionsExtension : IBloomFilterOptionsExtension
    {
        private readonly FilterCSRedisOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCSRedisOptionsExtension"/> class.
        /// </summary>
        /// <param name="options">Configure.</param>
        public FilterCSRedisOptionsExtension(FilterCSRedisOptions options)
        {
            _options = options;
        }

        public void AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<IBloomFilterFactory, DefaultBloomFilterFactory>();

            services.AddSingleton<IBloomFilter, FilterCSRedis>(x =>
            {
                FilterCSRedis createFilter(CSRedisClient client)
                {
                    return new FilterCSRedis(
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
                    throw new ArgumentException($"{nameof(FilterCSRedisOptions.ConnectionStrings)} is Empty!");
                }

                if (_options.Sentinels != null && _options.Sentinels.Any())
                {
                    var redisClient = new CSRedisClient(_options.ConnectionStrings[0], _options.Sentinels.ToArray(), _options.ReadOnly);
                    return createFilter(redisClient);
                }

                if (_options.ConnectionStrings.Count == 1)
                {
                    var redisClient = new CSRedisClient(_options.ConnectionStrings[0]);
                    return createFilter(redisClient);
                }
                else
                {
                    if (_options.NodeRule == null)
                    {
                        throw new ArgumentNullException(nameof(FilterCSRedisOptions.NodeRule));
                    }

                    var redisClient = new CSRedisClient(_options.NodeRule, _options.ConnectionStrings.ToArray());
                    return createFilter(redisClient);
                }
            });
        }
    }
}