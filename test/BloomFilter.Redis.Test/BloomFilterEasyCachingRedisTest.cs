using BloomFilter.CSRedis.Configurations;
using BloomFilter.EasyCaching;
using CSRedis;
using EasyCaching.Core.Serialization;
using EasyCaching.CSRedis;
using EasyCaching.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace BloomFilter.Redis.Test
{
    public class BloomFilterEasyCachingRedis
    {
        [Fact]
        public void RedisOptionsTest()
        {
            var services = new ServiceCollection();

            services.AddEasyCaching(setupAction =>
            {
                setupAction.WithJson();

                setupAction.UseCSRedis(configure =>
                {
                    configure.SerializerName = "json";
                    configure.DBConfig = new CSRedisDBOptions
                    {
                        ConnectionStrings = new System.Collections.Generic.List<string>
                        {
                            "127.0.0.1,defaultDatabase=0,poolsize=10"
                        }
                    };
                }, "BloomFilter1");

                setupAction.UseCSRedis(configure =>
                {
                    configure.SerializerName = "json";
                    configure.DBConfig = new CSRedisDBOptions
                    {
                        ConnectionStrings = new System.Collections.Generic.List<string>
                        {
                            "127.0.0.1,defaultDatabase=1,poolsize=10"
                        }
                    };
                }, "BloomFilter2");
            });

            services.AddBloomFilter(setupAction =>
            {
                setupAction.UseEasyCachingRedis(new FilterEasyCachingRedisOptions
                {
                    Name = "BF1",
                    RedisKey = "EasyCaching1",
                    ProviderName = "BloomFilter1"
                });

                //BloomFilter2
                setupAction.UseEasyCachingRedis(new FilterEasyCachingRedisOptions
                {
                    Name = "BF2",
                    RedisKey = "EasyCaching1",

                });
            });

            var provider = services.BuildServiceProvider();
            var factory = provider.GetService<IBloomFilterFactory>();

            Test(provider.GetService<IBloomFilter>());
            Test(factory.Get("BF1"));
            Test(factory.Get("BF2"));
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