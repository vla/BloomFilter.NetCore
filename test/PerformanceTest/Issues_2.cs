using BloomFilter;
using BloomFilter.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest
{
    public class Issues_2
    {
        [Test("Redis Performance")]
        public async Task Performance()
        {
            var bf = FilterRedisBuilder.Build("localhost", "bftest", 100000, 0.01);

            var items = new List<string>();

            var count = 50;

            var rndNum = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < count; i++)
            {
                items.Add(Helper.GenerateString(rndNum.Next(5, 30)));
            }

            await bf.AddAsync(items);

            var sw = Stopwatch.StartNew();

            await bf.ContainsAsync(items);

            Console.WriteLine(sw.Elapsed);
        }
    }
}