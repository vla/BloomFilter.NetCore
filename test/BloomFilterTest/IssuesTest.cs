using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BloomFilter;
using BloomFilter.Configurations;
using Xunit;

namespace BloomFilterTest
{
    public class IssuesTest
    {
        //Make sure your local memory is large enough
        [Theory]
        [InlineData(100_000_000)]
        [InlineData(1024 * 1024 * 1024)]
        [InlineData(2147483640)]
        [InlineData(2147483647)]
        [InlineData(2147483641)]
        [InlineData(2147483649)]
        [InlineData(2147483630)]
        public void Issues_16(long expectedElements)
        {
            double errorRate = 0.0000001;
            var hashMethod = HashMethod.XXHash128;

            var bf = (FilterMemory)FilterBuilder.Build(expectedElements, errorRate, hashMethod);
            var bytes = bf.ExportToBytes();

            var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions()
            {
                Method = hashMethod,
                ExpectedElements = expectedElements,
                ErrorRate = errorRate,
                BucketBytes = bytes
            });
        }
    }
}
