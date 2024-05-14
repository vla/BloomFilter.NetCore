using BloomFilter;
using System;
using System.Collections;
using Xunit;

namespace BloomFilterTest
{
    public class ImportExportTest
    {
        [Fact]
        public void Bytes_Import_OutOfRangeExceptione()
        {
            var bits1 = new byte[11981];

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var bf2 = (FilterMemory)FilterBuilder.Build(bits1, 10000, 0.01);
            });
        }

        [Fact]
        public void BitArray_Import_OutOfRangeException()
        {
            var bits1 = new BitArray(95850);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var bf2 = (FilterMemory)FilterBuilder.Build(bits1, 10000, 0.01);
            });
        }

        [Fact]
        public void Bytes_Export_And_Import()
        {
            var bf = (FilterMemory)FilterBuilder.Build(10000, 0.01);

            bf.Add("Export_And_Import");

            bf.Export(out byte[] bits, out byte[] more);

            var bf2 = (FilterMemory)FilterBuilder.Build(bits, more, 10000, 0.01);

            Assert.True(bf2.Contains("Export_And_Import"));
        }


        [Fact]
        public void BitArray_Export_And_Import()
        {
            var bf = (FilterMemory)FilterBuilder.Build(10000, 0.01);

            bf.Add("Export_And_Import");

            bf.Export(out BitArray bits, out BitArray more);

            var bf2 = (FilterMemory)FilterBuilder.Build(bits, more, 10000, 0.01);

            Assert.True(bf2.Contains("Export_And_Import"));
        }

    }
}
