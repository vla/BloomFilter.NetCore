using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BloomFilter;
using BloomFilter.Redis;

namespace BenchmarkTest
{
    [ClrJob, CoreJob]
    [MemoryDiagnoser]
    public class RedisBenchmark
    {
        private Filter<string> filter;
        private byte[] data;


        [Params(64)]
        public int DataSize;

        [GlobalSetup]
        public void Setup()
        {
            var n = 1000000;
            var errRate = 0.01;

            data = Helper.GenerateBytes(DataSize);
            filter = FilterRedisBuilder.Build<string>("localhost", "RedisBloomFilterTest", n, errRate,  HashMethod.Murmur3KirschMitzenmacher);
            filter.Clear();
        }

        [Benchmark]
        public void Add()
        {
            filter.Add(data);
        }
    }
}
