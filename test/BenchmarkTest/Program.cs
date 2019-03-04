using System;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace BenchmarkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = new BenchmarkSwitcher(typeof(Program).Assembly).Run(args);
            //var summary = BenchmarkRunner.Run<MemoryBenchmark>();
            Console.Read();
        }
    }
}
