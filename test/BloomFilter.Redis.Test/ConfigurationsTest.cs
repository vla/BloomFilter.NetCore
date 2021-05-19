using BloomFilter.Redis.Configurations;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Linq;
using Xunit;

namespace BloomFilter.Redis.Test
{
    public class ConfigurationsTest
    {
        [Fact]
        public void RedisOptionsTest()
        {
            var services = new ServiceCollection();

            services.AddBloomFilter(setupAction =>
            {
                setupAction.UseRedis(new FilterRedisOptions
                {
                    Name = "Redis1",
                    RedisKey = "BloomFilter1",
                    Endpoints = new[] { "localhost" }.ToList()
                });
            });

            var provider = services.BuildServiceProvider();

            Test(provider.GetService<IBloomFilter>());
        }

        [Fact]
        public void RedisOptionsConfigurationTest()
        {
            var services = new ServiceCollection();

            services.AddBloomFilter(setupAction =>
            {
                setupAction.UseRedis(new FilterRedisOptions
                {
                    Name = "Redis2",
                    RedisKey = "BloomFilter2",
                    Configuration = "localhost"
                });
            });

            var provider = services.BuildServiceProvider();

            Test(provider.GetService<IBloomFilter>());
        }

        [Fact]
        public void RedisOptionsSharedTest()
        {
            var services = new ServiceCollection();

            var connection = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse("localhost"));

            services.AddBloomFilter(setupAction =>
            {
                setupAction.UseRedis(new FilterRedisOptions
                {
                    Name = "Redis3",
                    RedisKey = "BloomFilter3",
                    Connection = connection
                });

                setupAction.UseRedis(new FilterRedisOptions
                {
                    Name = "Redis4",
                    RedisKey = "BloomFilter4",
                    Connection = connection
                });
            });

            var provider = services.BuildServiceProvider();
            var factory = provider.GetService<IBloomFilterFactory>();

            Test(provider.GetService<IBloomFilter>());
            Test(factory.Get("Redis3"));
            Test(factory.Get("Redis4"));
        }

        private void Test(IBloomFilter bf)
        {
            var len = 10;
            var array = new string[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = Utilitiy.GenerateString(10);
            }

            Assert.All(bf.Add(array), r => Assert.True(r));
            Assert.All(bf.Contains(array), r => Assert.True(r));
            Assert.True(bf.All(array));
            bf.Clear();
            Assert.All(bf.Contains(array), r => Assert.False(r));
            Assert.False(bf.All(array));
        }
    }
}