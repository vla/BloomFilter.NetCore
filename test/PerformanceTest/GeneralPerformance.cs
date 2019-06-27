using BloomFilter;
using BloomFilter.Redis;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest
{
    public class GeneralPerformance
    {

        [Test("RedisConcurrentPerformance")]
        public void RedisConcurrentPerformance()
        {
            var sw = Stopwatch.StartNew();

            var n = 3000000;
            var errRate = 0.01;

            var bf = FilterRedisBuilder.Build<string>("localhost", "bftest", n, errRate);

            var manual = new ManualResetEvent(false);

            var tasks = new Task[4];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                {
                    manual.WaitOne();
                    int count = 10000;
                    while (count-- > 0)
                    {
                        var data = Helper.GenerateBytes(12);
                        bf.Add(data);
                        if (!bf.Contains(data))
                        {
                            Console.WriteLine("match data!");
                        }
                    }

                });

                tasks[i].Start();
            }

            manual.Set();
            Task.WaitAll(tasks);

            sw.Stop();
            Console.WriteLine("Time" + sw.Elapsed);

            bf.Dispose();

        }

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
                    bf.Dispose();
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
