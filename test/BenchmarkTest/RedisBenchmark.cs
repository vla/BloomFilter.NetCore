using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BloomFilter;
using BloomFilter.Redis;

namespace BenchmarkTest
{
    //[
    //   SimpleJob(RuntimeMoniker.Net471),
    //   SimpleJob(RuntimeMoniker.NetCoreApp31, baseline: true)
    //]
    [MemoryDiagnoser]
    public class RedisBenchmark
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
            filter = FilterRedisBuilder.Build("localhost", "RedisBloomFilterTest", n, errRate, HashMethod.Murmur3KirschMitzenmacher);
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
