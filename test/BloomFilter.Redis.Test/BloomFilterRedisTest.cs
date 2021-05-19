using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BloomFilter;
using BloomFilter.Redis;
using StackExchange.Redis;
using Xunit;

namespace BloomFilter.Redis.Test
{
    public class BloomFilterRedisTest
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
            var bf = FilterRedisBuilder.Build("localhost", "NormalTest", 10000, 0.01, hashMethod);

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
        public async Task NormalTestAsync(HashMethod hashMethod)
        {
            var bf = FilterRedisBuilder.Build("localhost", "NormalTestAsync", 10000, 0.01, hashMethod);

            var len = 10;
            var array = new string[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = Utilitiy.GenerateString(10);
            }

            Assert.All(await bf.AddAsync(array), r => Assert.True(r));
            Assert.All(await bf.ContainsAsync(array), r => Assert.True(r));

            Assert.True(await bf.AllAsync(array));

            await bf.ClearAsync();

            Assert.All(await bf.ContainsAsync(array), r => Assert.False(r));
            Assert.False(await bf.AllAsync(array));
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
            var bf = FilterRedisBuilder.Build("localhost", "BytesArrayTest", 10000, 0.01, hashMethod);

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
        public void IntTest()
        {
            var config = ConfigurationOptions.Parse("localhost");

            var bf = FilterRedisBuilder.Build(config, "IntTest", 10000, 0.01);

            var len = 10;
            var array = new int[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = i;
            }

            Assert.All(bf.Add(array), r => Assert.True(r));
            Assert.All(bf.Contains(array), r => Assert.True(r));

            Assert.True(bf.All(array));

            bf.Clear();

            Assert.All(bf.Contains(array), r => Assert.False(r));
            Assert.False(bf.All(array));
        }


        [Fact]
        public async Task IntTestAsync()
        {
            var config = ConfigurationOptions.Parse("localhost");

            var bf = FilterRedisBuilder.Build(config, "IntTestAsync", 10000, 0.01);

            var len = 10;
            var array = new int[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = i;
            }

            Assert.All(await bf.AddAsync(array), r => Assert.True(r));
            Assert.All(await bf.ContainsAsync(array), r => Assert.True(r));

            Assert.True(await bf.AllAsync(array));

            await bf.ClearAsync();

            Assert.All(await bf.ContainsAsync(array), r => Assert.False(r));
            Assert.False(await bf.AllAsync(array));
        }


        [Fact]
        public void BuildTest()
        {
            var hashFun = new HashAlgorithms.HashCryptoSHA256();
            var config = ConfigurationOptions.Parse("localhost");
            var conn = ConnectionMultiplexer.Connect(config);
            var operate = new RedisBitOperate(conn);

            buildTest(FilterRedisBuilder.Build(config, "BuildTest", 10000));
            buildTest(FilterRedisBuilder.Build(config, "BuildTest", 10000, hashFun));
            buildTest(FilterRedisBuilder.Build(config, "BuildTest", 10000, HashMethod.Adler32));
            buildTest(FilterRedisBuilder.Build(config, "BuildTest", 10000, 0.01));
            buildTest(FilterRedisBuilder.Build(config, "BuildTest", 10000, 0.01, hashFun));
            buildTest(FilterRedisBuilder.Build(config, "BuildTest", 10000, 0.01, HashMethod.Adler32));

            buildTest(FilterRedisBuilder.Build("localhost", "BuildTest", 10000));
            buildTest(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, hashFun));
            buildTest(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, HashMethod.Adler32));
            buildTest(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, 0.01));
            buildTest(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, 0.01, hashFun));
            buildTest(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, 0.01, HashMethod.Adler32));

            buildTest(FilterRedisBuilder.Build(conn, "BuildTest", 10000));
            buildTest(FilterRedisBuilder.Build(conn, "BuildTest", 10000, hashFun));
            buildTest(FilterRedisBuilder.Build(conn, "BuildTest", 10000, HashMethod.Adler32));
            buildTest(FilterRedisBuilder.Build(conn, "BuildTest", 10000, 0.01));
            buildTest(FilterRedisBuilder.Build(conn, "BuildTest", 10000, 0.01, hashFun));
            buildTest(FilterRedisBuilder.Build(conn, "BuildTest", 10000, 0.01, HashMethod.Adler32));

            buildTest(FilterRedisBuilder.Build(operate, "BuildTest", 10000));
            buildTest(FilterRedisBuilder.Build(operate, "BuildTest", 10000, hashFun));
            buildTest(FilterRedisBuilder.Build(operate, "BuildTest", 10000, HashMethod.Adler32));
            buildTest(FilterRedisBuilder.Build(operate, "BuildTest", 10000, 0.01));
            buildTest(FilterRedisBuilder.Build(operate, "BuildTest", 10000, 0.01, hashFun));
            buildTest(FilterRedisBuilder.Build(operate, "BuildTest", 10000, 0.01, HashMethod.Adler32));


            conn.Dispose();
        }

        void buildTest(IBloomFilter bf)
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
        public async Task BuildTestAsync()
        {
            var hashFun = new HashAlgorithms.HashCryptoSHA256();
            var config = ConfigurationOptions.Parse("localhost");
            var conn = ConnectionMultiplexer.Connect(config);
            var operate = new RedisBitOperate(conn);

            await buildTestAsync(FilterRedisBuilder.Build(config, "BuildTest", 10000));
            await buildTestAsync(FilterRedisBuilder.Build(config, "BuildTest", 10000, hashFun));
            await buildTestAsync(FilterRedisBuilder.Build(config, "BuildTest", 10000, HashMethod.Adler32));
            await buildTestAsync(FilterRedisBuilder.Build(config, "BuildTest", 10000, 0.01));
            await buildTestAsync(FilterRedisBuilder.Build(config, "BuildTest", 10000, 0.01, hashFun));
            await buildTestAsync(FilterRedisBuilder.Build(config, "BuildTest", 10000, 0.01, HashMethod.Adler32));

            await buildTestAsync(FilterRedisBuilder.Build("localhost", "BuildTest", 10000));
            await buildTestAsync(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, hashFun));
            await buildTestAsync(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, HashMethod.Adler32));
            await buildTestAsync(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, 0.01));
            await buildTestAsync(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, 0.01, hashFun));
            await buildTestAsync(FilterRedisBuilder.Build("localhost", "BuildTest", 10000, 0.01, HashMethod.Adler32));

            await buildTestAsync(FilterRedisBuilder.Build(conn, "BuildTest", 10000));
            await buildTestAsync(FilterRedisBuilder.Build(conn, "BuildTest", 10000, hashFun));
            await buildTestAsync(FilterRedisBuilder.Build(conn, "BuildTest", 10000, HashMethod.Adler32));
            await buildTestAsync(FilterRedisBuilder.Build(conn, "BuildTest", 10000, 0.01));
            await buildTestAsync(FilterRedisBuilder.Build(conn, "BuildTest", 10000, 0.01, hashFun));
            await buildTestAsync(FilterRedisBuilder.Build(conn, "BuildTest", 10000, 0.01, HashMethod.Adler32));

            await buildTestAsync(FilterRedisBuilder.Build(operate, "BuildTest", 10000));
            await buildTestAsync(FilterRedisBuilder.Build(operate, "BuildTest", 10000, hashFun));
            await buildTestAsync(FilterRedisBuilder.Build(operate, "BuildTest", 10000, HashMethod.Adler32));
            await buildTestAsync(FilterRedisBuilder.Build(operate, "BuildTest", 10000, 0.01));
            await buildTestAsync(FilterRedisBuilder.Build(operate, "BuildTest", 10000, 0.01, hashFun));
            await buildTestAsync(FilterRedisBuilder.Build(operate, "BuildTest", 10000, 0.01, HashMethod.Adler32));


            conn.Dispose();
        }

        async Task buildTestAsync(IBloomFilter bf)
        {
            var len = 20;
            var array = new string[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = Utilitiy.GenerateString(10);
            }

            Assert.All(await bf.AddAsync(array), r => Assert.True(r));
            Assert.All(await bf.ContainsAsync(array), r => Assert.True(r));

            Assert.True(await bf.AllAsync(array));

            await bf.ClearAsync();

            Assert.All(await bf.ContainsAsync(array), r => Assert.False(r));
            Assert.False(await bf.AllAsync(array));
        }


    }
}
