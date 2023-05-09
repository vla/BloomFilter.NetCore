using BenchmarkDotNet.Attributes;
using BloomFilter;
using BloomFilter.FreeRedis;
using FreeRedis;
using System.Threading.Tasks;

namespace BenchmarkTest
{
    [RPlotExporter, RankColumn]
    [MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class FreeRedisBenchmark
    {
        private const int B = 64;
        private const int KB = 1024;
        private const int MB = 1024 * KB;

        private IBloomFilter filter;
        private byte[] data;

        [Params(B, KB, MB)]
        public int DataSize;

        [GlobalSetup]
        public void Setup()
        {
            uint n = 1000000;
            var errRate = 0.01;

            data = Helper.GenerateBytes(DataSize);
            var client = new RedisClient("localhost");
            filter = new FilterFreeRedis(
                        BloomFilterConstValue.DefaultRedisName,
                        client,
                        "BloomFilter.Core",
                        n,
                        errRate,
                        HashFunction.Functions[HashMethod.Murmur3]);
            filter.Clear();
        }

        [Benchmark]
        public void Add()
        {
            filter.Add(data);
        }

        [Benchmark]
        public async ValueTask AddAsync()
        {
            await filter.AddAsync(data);
        }
    }
}