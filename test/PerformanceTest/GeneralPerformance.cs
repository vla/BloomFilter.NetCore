using BloomFilter;
using BloomFilter.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

            var names = Enum.GetNames(typeof(HashMethod));
            foreach (var name in names)
            {
                if (Enum.TryParse<HashMethod>(name, out var hm))
                {
                    var bf = FilterRedisBuilder.Build<string>("localhost", "bftest", n, errRate, hm);
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
                    var bf = FilterRedisBuilder.Build<string>(configuration, "bftest", n, errRate, hm);
                    bf.Clear();
                    Console.WriteLine($"=================== {name} Performance =================== ");
                    Performance(hashData, bf);
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
                    var bf = FilterBuilder.Build<string>(n, errRate, hm);
                    Console.WriteLine($"=================== {name} Performance =================== ");
                    Performance(hashData, bf);
                }
            }
        }

        private void Performance(IList<byte[]> hashData, Filter<string> bf)
        {
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var data in hashData)
            {
                bf.Add(data);
            }
            sw.Stop();

            Console.WriteLine($"Added Speed {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            foreach (var data in hashData)
            {
                bf.Contains(data);
            }
            sw.Stop();
            Console.WriteLine($"Contains Speed {sw.ElapsedMilliseconds}ms");
        }
    }
}
