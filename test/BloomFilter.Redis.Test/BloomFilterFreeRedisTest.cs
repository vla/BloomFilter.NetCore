using BloomFilter.FreeRedis.Configurations;
using FreeRedis;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace BloomFilter.Redis.Test
{
    public class BloomFilterFreeRedisTest
    {
        [Fact]
        public void RedisOptionsTest()
        {
            var services = new ServiceCollection();

            services.AddBloomFilter(setupAction =>
            {
                setupAction.UseFreeRedis(new FilterFreeRedisOptions
                {
                    Name = "Redis1",
                    RedisKey = "FreeRedis1",
                    ConnectionStrings = new[] { "localhost" }.ToList()
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
                setupAction.UseFreeRedis(new FilterFreeRedisOptions
                {
                    Name = "Redis2",
                    RedisKey = "FreeRedis2",
                    ConnectionStrings = new[] { "localhost" }.ToList()
                });
            });

            var provider = services.BuildServiceProvider();

            Test(provider.GetService<IBloomFilter>());
        }

        [Fact]
        public void RedisOptionsSharedTest()
        {
            var services = new ServiceCollection();

            var clint = new RedisClient("localhost");

            services.AddBloomFilter(setupAction =>
            {
                setupAction.UseFreeRedis(new FilterFreeRedisOptions
                {
                    Name = "Redis3",
                    RedisKey = "FreeRedis3",
                    Client = clint
                });

                setupAction.UseFreeRedis(new FilterFreeRedisOptions
                {
                    Name = "Redis4",
                    RedisKey = "FreeRedis4",
                    Client = clint
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