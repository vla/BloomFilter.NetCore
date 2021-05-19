using BloomFilter;
using BloomFilter.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTest
{
    public class GeneralPerformance
    {
        [Test("RedisPerformance")]
        public void RedisPerformance()
        {
            var n = 30000;
            var errRate = 0.01;

            var hashData = Helper.GenerateData(n);

            var warm_up = FilterRedisBuilder.Build("localhost", "RedisPerformance", n, errRate);
            warm_up.Clear();
            Console.WriteLine($"=================== warm_up Performance =================== ");
            Performance(hashData, warm_up);

            var names = Enum.GetNames(typeof(HashMethod));
            foreach (var name in names)
            {
                if (Enum.TryParse<HashMethod>(name, out var hm))
                {
                    var bf = FilterRedisBuilder.Build("localhost", "RedisPerformance", n, errRate, hm);
                    bf.Clear();
                    Console.WriteLine($"=================== {name} Performance =================== ");
                    Performance(hashData, bf);
                }
            }
        }

        [Test("RedisClusterPerformance")]
        public void RedisClusterPerformance()
        {
            var n = 30000;
            var errRate = 0.01;

            var hashData = Helper.GenerateData(n);

            var configuration = "127.0.0.1:7000,127.0.0.1:7001,127.0.0.1:7002,127.0.0.1:7003,127.0.0.1:7004,127.0.0.1:7005";

            var conn = ConnectionMultiplexer.Connect(configuration);
            foreach (var entry in conn.GetEndPoints())
            {
                var server = conn.GetServer(entry);
                Console.WriteLine(server.EndPoint + " " + server.ServerType);
            }
            conn.Dispose();

            var names = Enum.GetNames(typeof(HashMethod));
            foreach (var name in names)
            {
                if (Enum.TryParse<HashMethod>(name, out var hm))
                {
                    var bf = FilterRedisBuilder.Build(configuration, "bftest", n, errRate, hm);
                    bf.Clear();
                    Console.WriteLine($"=================== {name} Performance =================== ");
                    Performance(hashData, bf);
                    bf.Dispose();
                }
            }
        }

        [Test("MemoryPerformance")]
        public void MemoryPerformance()
        {
            var n = 30000;
            var errRate = 0.01;

            var hashData = Helper.GenerateData(n);

            var names = Enum.GetNames(typeof(HashMethod));
            foreach (var name in names)
            {
                if (Enum.TryParse<HashMethod>(name, out var hm))
                {
                    var bf = FilterBuilder.Build(n, errRate, hm);
                    Console.WriteLine($"=================== {name} Performance =================== ");
                    Performance(hashData, bf);
                }
            }
        }

        private void Performance(IList<byte[]> hashData, IBloomFilter bf)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int index = 0;
            int count = hashData.Count;

            Parallel.ForEach(hashData, (data) =>
            {
                bf.Add(data);
                Interlocked.Increment(ref index);
            });

            while (index < count) { }

            sw.Stop();

            Console.WriteLine($"Added Speed {sw.ElapsedMilliseconds}ms");

            sw.Restart();

            bool hasErr = false;

            int index2 = 0;

            var ret = Parallel.ForEach(hashData, (data) =>
            {
                var hasDta = bf.Contains(data);
                Interlocked.Increment(ref index2);
                if (!hasDta)
                {
                    if (hasErr)
                        return;
                    hasErr = true;
                    Console.WriteLine("Error Match!");
                }
            });

            while (index2 < count) { }

            sw.Stop();
            Console.WriteLine($"Contains Speed {sw.ElapsedMilliseconds}ms");

            bf.Dispose();
        }
    }
}