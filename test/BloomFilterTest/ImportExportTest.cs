using BloomFilter;
using System;
using System.Collections;
using Xunit;
using BloomFilter.Configurations;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BloomFilterTest
{
    public class ImportExportTest
    {

        [Fact]
        public void ExportToBytes_And_Import()
        {
            var bf = (FilterMemory)FilterBuilder.Build(10000, 0.01);

            bf.Add("ExportToBytes_And_Import");

            var bucketBytes = bf.ExportToBytes();

            var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions
            {
                BucketBytes = bucketBytes,
                ExpectedElements = 10000,
                ErrorRate = 0.01
            });


            Assert.True(bf2.Contains("ExportToBytes_And_Import"));
        }

        [Fact]
        public void Export_BitArray_And_Import()
        {
            var bf = (FilterMemory)FilterBuilder.Build(10000, 0.01);

            bf.Add("Export_BitArray_And_Import");

            var buckets = bf.Export();

            var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions
            {
                Buckets = buckets,
                ExpectedElements = 10000,
                ErrorRate = 0.01
            });


            Assert.True(bf2.Contains("Export_BitArray_And_Import"));
        }

        ////Make sure your local memory is large enough
        [Theory]
        [InlineData(4_000)]
        [InlineData(100_000_000)]
        [InlineData(1024 * 1024 * 1024)]
        [InlineData(2147483640)]
        [InlineData(2147483641)]
        [InlineData(2147483647)]
        [InlineData(2147483649)]
        [InlineData(2147483630)]
        public void Fill_Export_And_Import_Contains(long expectedElements)
        {
            double errorRate = 0.1;
            var hashMethod = HashMethod.XXHash128;

            var bf = (FilterMemory)FilterBuilder.Build(expectedElements, errorRate, hashMethod);

            var rng = RandomNumberGenerator.Create();

            var len = 1000;
            var list = new List<byte[]>(len);
            for (int i = 0; i < len; i++)
            {
                var data = new byte[1024];
                rng.GetBytes(data);
                list.Add(data);
            }

            //fill
            Assert.All(bf.Add(list), r => Assert.True(r));

            var bytes = bf.ExportToBytes();

            var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions()
            {
                Method = hashMethod,
                ExpectedElements = expectedElements,
                ErrorRate = errorRate,
                BucketBytes = bytes
            });
        
            //contains
            Assert.All(bf2.Contains(list), r => Assert.True(r));

            Assert.True(bf2.All(list));
        }

    }
}
