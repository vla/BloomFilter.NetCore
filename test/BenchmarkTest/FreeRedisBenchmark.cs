using BenchmarkDotNet.Attributes;
using BloomFilter;
using BloomFilter.FreeRedis;
using FreeRedis;
using System.Threading.Tasks;

namespace BenchmarkTest
{
    [MemoryDiagnoser]
    public class FreeRedisBenchmark
    {
        private IBloomFilter filter;
        private byte[] data;

        [Params(64)]
        public int DataSize;

        [GlobalSetup]
        public void Setup()
        {
            var n = 1000000;
            var errRate = 0.01;

            data = Helper.GenerateBytes(DataSize);
            var client = new RedisClient("localhost");
            filter = new FilterFreeRedis(
                        BloomFilterConstValue.DefaultRedisName,
                        client,
                        "BloomFilter.Core",
                        n,
                        errRate,
                        HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
            filter.Clear();
        }

        [Benchmark]
        public void Add()
        {
            filter.Add(data);
        }

        [Benchmark]
        public Task AddAsync()
        {
            return filter.AddAsync(data);
        }
    }
}