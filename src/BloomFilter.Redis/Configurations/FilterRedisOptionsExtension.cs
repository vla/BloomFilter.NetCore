using BloomFilter.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace BloomFilter.Redis.Configurations
{
    public class FilterRedisOptionsExtension : IBloomFilterOptionsExtension
    {
        private readonly FilterRedisOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRedisOptionsExtension"/> class.
        /// </summary>
        /// <param name="options">Configure.</param>
        public FilterRedisOptionsExtension(FilterRedisOptions options)
        {
            _options = options;
        }

        public void AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<IBloomFilterFactory, DefaultBloomFilterFactory>();

            services.AddSingleton<IBloomFilter, FilterRedis>(x =>
            {
                IRedisBitOperate redisBitOperate;

                if (_options.Connection != null)
                {
                    redisBitOperate = new RedisBitOperate(_options.Connection);
                }
                else if (!string.IsNullOrWhiteSpace(_options.Configuration))
                {
                    redisBitOperate = new RedisBitOperate(_options.Configuration!);
                }
                else
                {
                    var configurationOptions = new ConfigurationOptions
                    {
                        ConnectTimeout = _options.ConnectionTimeout,
                        User = _options.Username,
                        Password = _options.Password,
                        Ssl = _options.IsSsl,
                        SslHost = _options.SslHost,
                        AllowAdmin = _options.AllowAdmin,
                        DefaultDatabase = _options.Database,
                        AbortOnConnectFail = _options.AbortOnConnectFail,
                    };

                    foreach (var endpoint in _options.Endpoints)
                    {
                        configurationOptions.EndPoints.Add(endpoint);
                    }

                    redisBitOperate = new RedisBitOperate(configurationOptions);
                }

                return new FilterRedis(
                      _options.Name,
                      redisBitOperate,
                      _options.RedisKey,
                      _options.ExpectedElements,
                      _options.ErrorRate,
                      HashFunction.Functions[_options.Method]);
            });
        }
    }
}