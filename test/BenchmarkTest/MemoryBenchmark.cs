using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BloomFilter;

namespace BenchmarkTest
{
    [MemoryDiagnoser]
    public class MemoryBenchmark
    {
        private IBloomFilter filter;
        private byte[] data;

        [ParamsAllValues]
        public HashMethod Method;

        [Params(64)]
        public int DataSize;

        [GlobalSetup]
        public void Setup()
        {
            var n = 1000000;
            var errRate = 0.01;

            data = Helper.GenerateBytes(DataSize);
            filter = FilterBuilder.Build(n, errRate, Method);
        }

        [Benchmark]
        public void Add()
        {
            filter.Add(data);
        }
    }
}
