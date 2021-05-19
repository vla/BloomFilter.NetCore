using BloomFilter;
using BloomFilter.HashAlgorithms;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
        [InlineData(HashMethod.SHA1)]
        [InlineData(HashMethod.SHA256)]
        [InlineData(HashMethod.SHA384)]
        [InlineData(HashMethod.SHA512)]
        public void NormalTest(HashMethod hashMethod)
        {
            var bf = FilterBuilder.Build(10000, 0.01, hashMethod);

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
        [InlineData(HashMethod.SHA1)]
        [InlineData(HashMethod.SHA256)]
        [InlineData(HashMethod.SHA384)]
        [InlineData(HashMethod.SHA512)]
        public void BytesArrayTest(HashMethod hashMethod)
        {
            var bf = FilterBuilder.Build(10000, 0.01, hashMethod);

            var rng = RandomNumberGenerator.Create();

            var len = 10;
            var list = new List<byte[]>(len);
            for (int i = 0; i < len; i++)
            {
                var data = new byte[1024];
                rng.GetBytes(data);
                list.Add(data);
            }

            Assert.All(bf.Add(list), r => Assert.True(r));
            Assert.All(bf.Contains(list), r => Assert.True(r));

            Assert.True(bf.All(list));

            bf.Clear();

            Assert.All(bf.Contains(list), r => Assert.False(r));
            Assert.False(bf.All(list));
        }

        [Fact]
        public void Can_DataType_Exception_Constraint()
        {
            Assert.Throws<NotSupportedException>(() =>
            {
#pragma warning disable CS0618
                FilterBuilder.Build<BloomFilterTest>(10000, 0.01);
#pragma warning restore CS0618
            });

            Assert.Throws<NotSupportedException>(() =>
            {
#pragma warning disable CS0618
                FilterBuilder.Build<object>(10000, 0.01);
#pragma warning restore CS0618
            });
        }

        [Fact]
        public void BuildTest()
        {
            var hashFun = new HashChecksumCrc32();
            buildTest(FilterBuilder.Build(10000));
            buildTest(FilterBuilder.Build(10000, hashFun));
            buildTest(FilterBuilder.Build(10000, HashMethod.Adler32));
            buildTest(FilterBuilder.Build(10000, 0.01));
            buildTest(FilterBuilder.Build(10000, 0.01, hashFun));
            buildTest(FilterBuilder.Build(10000, 0.01, HashMethod.Adler32));
        }

        private void buildTest(IBloomFilter bf)
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

        [Fact]
        public void FixMurmur2Test()
        {
            var bf = FilterBuilder.Build(10000, 0.01, HashMethod.Murmur2);

            var rng = RandomNumberGenerator.Create();

            var data = new byte[1024];
            rng.GetBytes(data);

            Assert.True(bf.Add(data));

            Assert.True(bf.Contains(data));
        }
    }
}