using BloomFilter.Configurations;
using BloomFilter.EasyCaching;
using EasyCaching.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BloomFilter.CSRedis.Configurations
{
    public class FilterEasyCachingRedisExtension : IBloomFilterOptionsExtension
    {
        private readonly FilterEasyCachingRedisOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEasyCachingRedisExtension"/> class.
        /// </summary>
        /// <param name="options">Configure.</param>
        public FilterEasyCachingRedisExtension(FilterEasyCachingRedisOptions options)
        {
            _options = options;
        }

        public void AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<IBloomFilterFactory, DefaultBloomFilterFactory>();

            services.AddSingleton<IBloomFilter, FilterEasyCachingRedis>(x =>
            {
                var provider = x.GetRequiredService<IRedisCachingProvider>();

                if (!string.IsNullOrWhiteSpace(_options.ProviderName))
                {
                    var factory = x.GetRequiredService<IEasyCachingProviderFactory>();
                    provider = factory.GetRedisProvider(_options.ProviderName);
                }

                return new FilterEasyCachingRedis(
                        _options.Name,
                        provider,
                        _options.RedisKey,
                        _options.ExpectedElements,
                        _options.ErrorRate,
                        HashFunction.Functions[_options.Method]);
            });
        }
    }
}