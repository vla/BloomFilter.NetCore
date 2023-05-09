using BloomFilter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PerformanceTest
{
    public class HashErrRate
    {
        [Test("Bloom Filter Hash ErrRate")]
        public void ErrRateTest()
        {
            uint n = 100000;
            var errRate = 0.01;

            var hashData = Helper.GenerateData(n);
            var probeData = Helper.GenerateData(n * 3);

            Filter bf = FilterBuilder.Build(n, errRate) as Filter;
            Console.WriteLine($"Count:{n} Capacity:{bf.Capacity},Hashes:{bf.Hashes},ExpectedElements:{bf.ExpectedElements},ErrorRate:{bf.ErrorRate}");
            Console.WriteLine("---");

            var names = Enum.GetNames(typeof(HashMethod));
            foreach (var name in names)
            {
                if (Enum.TryParse<HashMethod>(name, out var hm))
                {
                    ErrRateTest(hm, hashData, probeData, n, errRate);
                }
            }
        }

        private void ErrRateTest(HashMethod hashMethod, IList<byte[]> hashData, IList<byte[]> probeData, uint n, double p)
        {

            var bf = FilterBuilder.Build(n, p, hashMethod);

            int inserts = hashData.Count;
            int errRates = 0;
            var set = new HashSet<byte[]>();

            Stopwatch sw = Stopwatch.StartNew();
            foreach (var data in hashData)
            {
                if (bf.Contains(data) && !set.Contains(data))
                {
                    errRates++;
                }
                bf.Add(data);
                set.Add(data);
            }
            sw.Stop();

            int totalErrRates = 0;
            int probed = 0;
            foreach (var data in probeData)
            {
                if (probed >= inserts)
                    break;
                if (!set.Contains(data))
                {
                    probed++;
                    if (bf.Contains(data))
                    {
                        totalErrRates++;
                    }
                }
            }

            double errorRate = 100.0 * errRates / inserts;
            double totalErrorRate = 100.0 * totalErrRates / inserts;

            Console.WriteLine($"{hashMethod}");
            Console.WriteLine($" Speed:{sw.Elapsed.TotalMilliseconds}ms ErrRate:{errorRate:F3}% ErrTotal:{errRates} Final ErrRate:{totalErrorRate:F3}%");
        }
    }
}
