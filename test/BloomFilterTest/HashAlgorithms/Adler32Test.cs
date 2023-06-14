using System;
using System.Linq;
using BloomFilter.HashAlgorithms.Internal;
using Xunit;

namespace BloomFilterTest.HashAlgorithms
{
    public class Adler32Test
    {

        [Theory]
        [InlineData(1024, 3838443024)]
        [InlineData(1024 * 1024, 1185183625)]
        [InlineData(32 * 1024 * 1024, 2524836307)]
        [InlineData(128 * 1024 * 1024, 4178691958)]
        [InlineData(363898432, 3303655557)]
        [InlineData(1024 * 1024 * 1024, 3590863875)]
        public void Adler_Append(int size, uint expected)
        {
            var bytes = Enumerable.Range(0, size).Select(i => (byte)i).ToArray();

            Span<byte> data = bytes;

            var a2 = new Adler32();

            while (data.Length > 0)
            {
                var n = Random.Shared.Next(1, data.Length);
                var d = data.Slice(0, n);
                a2.Append(d);
                data = data.Slice(n);
            }

            uint actual = a2.GetCurrentHashAsUInt32();

            Assert.Equal(expected, actual);
        }
    }
}