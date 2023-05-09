using BenchmarkDotNet.Attributes;
using BloomFilter;

namespace BenchmarkTest
{
    [RPlotExporter, RankColumn]
    [MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class MemoryBenchmark
    {
        private const int B = 64;
        private const int KB = 1024;
        private const int MB = 1024 * KB;

        private IBloomFilter filter;
        private byte[] data;

        [ParamsAllValues]
        public HashMethod Method;

        [Params(B, KB, MB)]
        public int DataSize;

        [GlobalSetup]
        public void Setup()
        {
            data = Helper.GenerateBytes(DataSize);

            uint n = 1000000;
            var errRate = 0.01;

            filter = FilterBuilder.Build(n, errRate, Method);
        }

        [Benchmark]
        public void Add()
        {
            filter.Add(data);
        }
    }
}