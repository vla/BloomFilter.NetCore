using System;
using System.IO;
using System.Threading.Tasks;
using BloomFilter;
using BloomFilter.Configurations;
using Xunit;

namespace BloomFilterTest
{
    public class SerializerTest
    {
        [Theory]
        [InlineData(10_000)]
        [InlineData(500_000)]
        public async Task Serialize(long expectedElements)
        {
            double errorRate = 0.0000001;
            var bf = (FilterMemory)FilterBuilder.Build(expectedElements, errorRate);

            bf.Add("ExportToBytes_And_Import");

            using var ms = new MemoryStream();

            await bf.SerializeAsync(ms);

            ms.Position = 0;

            var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions
            {
                ExpectedElements = 10000,
                ErrorRate = 0.01
            });

            await bf2.DeserializeAsync(ms);

            Assert.True(bf2.Contains("ExportToBytes_And_Import"));
            Assert.False(bf2.Contains("ExportToBytes_And_Import2"));

            Assert.Equal(bf.ExpectedElements, bf2.ExpectedElements);
            Assert.Equal(bf.ErrorRate, bf2.ErrorRate);

            // Check if the length of the stream is equal to the expected length
            using var ms2 = new MemoryStream();

            await bf2.SerializeAsync(ms2);

            Assert.Equal(ms.Length, ms2.Length);

        }

        //Make sure your local memory is large enough
        [Theory]
        [InlineData(100_000_000)]
        //[InlineData(1024 * 1024 * 1024)]
        //[InlineData(2147483640)]
        //[InlineData(2147483647)]
        //[InlineData(2147483641)]
        //[InlineData(2147483649)]
        //[InlineData(2147483630)]
        public async Task SerializeLarge(long expectedElements)
        {
            double errorRate = 0.0000001;
            var bf = (FilterMemory)FilterBuilder.Build(expectedElements, errorRate);

            bf.Add("ExportToBytes_And_Import");

            using var ms = new MemoryStream();

            await bf.SerializeAsync(ms);

            ms.Position = 0;

            var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions
            {
                ExpectedElements = 10000,
                ErrorRate = 0.01
            });

            await bf2.DeserializeAsync(ms);

            Assert.True(bf2.Contains("ExportToBytes_And_Import"));
            Assert.False(bf2.Contains("ExportToBytes_And_Import2"));
        }

    }
}
