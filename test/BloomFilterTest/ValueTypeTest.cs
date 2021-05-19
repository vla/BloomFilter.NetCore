using BloomFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BloomFilterTest
{
    public class ValueTypeTest
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
        public void Int32(HashMethod hashMethod)
        {
            var bf = FilterBuilder.Build(100000, 0.01, hashMethod);
            var len = 100;
            var list = new List<int>(len);
            for (int i = 0; i < len; i++)
            {
                list.Add(i);
            }

            Assert.All(bf.Add(list), r => Assert.True(r));
            Assert.All(bf.Contains(list), r => Assert.True(r));
            Assert.True(bf.Contains(list[0]));
            Assert.True(bf.All(list));

            bf.Clear();

            Assert.All(bf.Contains(list), r => Assert.False(r));
            Assert.False(bf.Contains(list[0]));
            Assert.False(bf.All(list));
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
        public void Int16(HashMethod hashMethod)
        {
            var bf = FilterBuilder.Build(100000, 0.01, hashMethod);
            var len = 100;
            var list = new List<short>(len);
            for (short i = 0; i < len; i++)
            {
                list.Add(i);
            }

            Assert.All(bf.Add(list), r => Assert.True(r));
            Assert.All(bf.Contains(list), r => Assert.True(r));
            Assert.True(bf.Contains(list[0]));
            Assert.True(bf.All(list));

            bf.Clear();

            Assert.All(bf.Contains(list), r => Assert.False(r));
            Assert.False(bf.Contains(list[0]));
            Assert.False(bf.All(list));
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
        public void Int64(HashMethod hashMethod)
        {
            var bf = FilterBuilder.Build(100000, 0.01, hashMethod);
            var len = 100;
            var list = new List<long>(len);
            for (long i = 0; i < len; i++)
            {
                list.Add(i);
            }

            Assert.All(bf.Add(list), r => Assert.True(r));
            Assert.All(bf.Contains(list), r => Assert.True(r));
            Assert.True(bf.Contains(list[0]));
            Assert.True(bf.All(list));

            bf.Clear();

            Assert.All(bf.Contains(list), r => Assert.False(r));
            Assert.False(bf.Contains(list[0]));
            Assert.False(bf.All(list));
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
        public void Double(HashMethod hashMethod)
        {
            var bf = FilterBuilder.Build(100000, 0.01, hashMethod);
            var len = 100;
            var list = new List<double>(len);
            for (var i = 0; i < len; i++)
            {
                list.Add(i * 99.99D);
            }

            Assert.All(bf.Add(list), r => Assert.True(r));
            Assert.All(bf.Contains(list), r => Assert.True(r));
            Assert.True(bf.Contains(list[0]));
            Assert.True(bf.All(list));

            bf.Clear();

            Assert.All(bf.Contains(list), r => Assert.False(r));
            Assert.False(bf.Contains(list[0]));
            Assert.False(bf.All(list));
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
        public void Float(HashMethod hashMethod)
        {
            var bf = FilterBuilder.Build(100000, 0.01, hashMethod);
            var len = 100;
            var list = new List<float>(len);
            for (var i = 0; i < len; i++)
            {
                list.Add(i * 99.99F);
            }

            Assert.All(bf.Add(list), r => Assert.True(r));
            Assert.All(bf.Contains(list), r => Assert.True(r));
            Assert.True(bf.Contains(list[0]));
            Assert.True(bf.All(list));

            bf.Clear();

            Assert.All(bf.Contains(list), r => Assert.False(r));
            Assert.False(bf.Contains(list[0]));
            Assert.False(bf.All(list));
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
        public void Date(HashMethod hashMethod)
        {
            var bf = FilterBuilder.Build(100000, 0.01, hashMethod);
            var len = 100;
            var list = new List<DateTime>(len);
            var now = DateTime.Now;
            for (var i = 0; i < len; i++)
            {
                list.Add(now.AddSeconds(i));
            }

            Assert.All(bf.Add(list), r => Assert.True(r));
            Assert.All(bf.Contains(list), r => Assert.True(r));
            Assert.True(bf.Contains(list[0]));
            Assert.True(bf.All(list));

            bf.Clear();

            Assert.All(bf.Contains(list), r => Assert.False(r));
            Assert.False(bf.Contains(list[0]));
            Assert.False(bf.All(list));
        }

    }
}
