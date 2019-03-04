using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BloomFilter;

namespace BenchmarkTest
{
    [ClrJob, CoreJob]
    [MemoryDiagnoser]
    public class MemoryBenchmark
    {
        private Filter<string> filter;
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
            filter = FilterBuilder.Build<string>(n, errRate, Method);
        }

        [Benchmark]
        public void Add()
        {
            filter.Add(data);
        }
    }
}
