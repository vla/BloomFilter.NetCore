using System;
using BloomFilter;
using BloomFilter.HashAlgorithms;
using Xunit;

namespace BloomFilterTest
{
    public class BloomFilterTest
    {
        [Theory]
        [InlineData(HashMethod.LCGWithFNV1)]
        [InlineData(HashMethod.LCGWithFNV1a)]
        [InlineData(HashMethod.LCGModifiedFNV1)]
        [InlineData(HashMethod.RNGWithFNV1)]
        [InlineData(HashMethod.RNGWithFNV1a)]
        [InlineData(HashMethod.RNGModifiedFNV1)]
        [InlineData(HashMethod.CRC32)]
        [InlineData(HashMethod.Adler32)]
        [InlineData(HashMethod.Murmur2)]
        [InlineData(HashMethod.Murmur3)]
        [InlineData(HashMethod.Murmur3KirschMitzenmacher)]
        [InlineData(HashMethod.MD5)]
        [InlineData(HashMethod.SHA1)]
        [InlineData(HashMethod.SHA256)]
        [InlineData(HashMethod.SHA384)]
        [InlineData(HashMethod.SHA512)]
        public void NormalTest(HashMethod hashMethod)
        {
            var bf = FilterBuilder.Build<string>(10000, 0.01, hashMethod);

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
        public void Can_DataType_Exception_Constraint()
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                FilterBuilder.Build<BloomFilterTest>(10000, 0.01);
            });

            Assert.Throws<NotSupportedException>(() =>
            {
                FilterBuilder.Build<object>(10000, 0.01);
            });
        }

        [Fact]
        public void BuildTest()
        {
            var hashFun = new HashChecksumCrc32();
            buildTest(FilterBuilder.Build<string>(10000));
            buildTest(FilterBuilder.Build<string>(10000, hashFun));
            buildTest(FilterBuilder.Build<string>(10000, HashMethod.Adler32));
            buildTest(FilterBuilder.Build<string>(10000, 0.01));
            buildTest(FilterBuilder.Build<string>(10000, 0.01, hashFun));
            buildTest(FilterBuilder.Build<string>(10000, 0.01, HashMethod.Adler32));


        }

        void buildTest(Filter<string> bf)
        {
            var len = 20;
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
