using BloomFilter;
using System;
using System.Collections;
using Xunit;
using BloomFilter.Configurations;

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
                var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions
                {
                    Bytes = bits1,
                    ExpectedElements = 10000,
                    ErrorRate = 0.01
                });
            });
        }

        [Fact]
        public void BitArray_Import_OutOfRangeException()
        {
            var bits1 = new BitArray(95850);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions
                {
                    Bits = bits1,
                    ExpectedElements = 10000,
                    ErrorRate = 0.01
                });
            });
        }

        [Fact]
        public void Bytes_Export_And_Import()
        {
            var bf = (FilterMemory)FilterBuilder.Build(10000, 0.01);

            bf.Add("Export_And_Import");

            bf.Export(out byte[] bits, out byte[] more);

            var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions
            {
                Bytes = bits,
                BytesMore = more,
                ExpectedElements = 10000,
                ErrorRate = 0.01
            });


            Assert.True(bf2.Contains("Export_And_Import"));
        }

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
        public void BitArray_Export_And_Import()
        {
            var bf = (FilterMemory)FilterBuilder.Build(10000, 0.01);

            bf.Add("Export_And_Import");

            bf.Export(out BitArray bits, out BitArray more);

            var bf2 = (FilterMemory)FilterBuilder.Build(new FilterMemoryOptions
            {
                Bits = bits,
                BitsMore = more,
                ExpectedElements = 10000,
                ErrorRate = 0.01
            });


            Assert.True(bf2.Contains("Export_And_Import"));
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

    }
}
