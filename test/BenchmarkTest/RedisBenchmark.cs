using BenchmarkDotNet.Attributes;
using BloomFilter;
using BloomFilter.Redis;
using System.Threading.Tasks;

namespace BenchmarkTest
{
    [RPlotExporter, RankColumn]
    [MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class RedisBenchmark
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
            filter = FilterRedisBuilder.Build("localhost", "RedisBloomFilterTest", n, errRate, HashMethod.Murmur3);
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