using BloomFilter;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BloomFilterTest
{
    public class ConfigurationsTest
    {
        [Fact]
        public void NormalTest()
        {
            var services = new ServiceCollection();

            services.AddBloomFilter(setupAction =>
            {
                setupAction.UseInMemory();
            });

            var provider = services.BuildServiceProvider();

            var bf = provider.GetService<IBloomFilter>();

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

        [Fact]
        public void BloomFilterFactoryTest()
        {
            var services = new ServiceCollection();

            services.AddBloomFilter(setupAction =>
            {
                setupAction.UseInMemory();
                setupAction.UseInMemory("CustomName");
            });

            var provider = services.BuildServiceProvider();

            var factory = provider.GetService<IBloomFilterFactory>();
            var bf1 = factory.Get(BloomFilterConstValue.DefaultInMemoryName);
            var bf2 = factory.Get("CustomName");
            var bf3 = provider.GetService<IBloomFilter>();


            Assert.Equal(BloomFilterConstValue.DefaultInMemoryName, bf1.Name);
            Assert.Equal("CustomName", bf2.Name);
            Assert.Equal("CustomName", bf3.Name);
        }
    }
}
