using System;
using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace BenchmarkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = new BenchmarkSwitcher(typeof(Program).Assembly).Run(args,
                 ManualConfig
                    .Create(DefaultConfig.Instance)
                    .WithOptions(ConfigOptions.JoinSummary | ConfigOptions.DisableLogFile | ConfigOptions.DisableOptimizationsValidator));
            //var summary = BenchmarkRunner.Run<MemoryBenchmark>();
            Console.Read();
        }
    }
}
