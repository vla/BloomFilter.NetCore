using BloomFilter.Redis;
using BloomFilter;
using System;

namespace Demo
{
    public class BloomFilterRedis
    {
        [Test("Sample In Redis")]
        public void SampleInRedis()
        {

            var names = Enum.GetNames(typeof(HashMethod));
            foreach (var name in names)
            {
                if (Enum.TryParse<HashMethod>(name, out var hm))
                {
                    var bf = FilterRedisBuilder.Build("localhost", "bftest", 5000000, 0.001, hm);
                    Sample(bf);
                    bf.Dispose();
                }
            }
        }


        private void Sample(IBloomFilter bf)
        {

            bf.Add("CAR_CAR_LOG1ssd3");
            Console.WriteLine(bf.Contains("CAR_CAR_LOG1ssd3"));

            bf.Clear();
            Console.WriteLine(bf.Contains("CAR_CAR_LOG1ssd3"));

            bf.Add("CAR_CAR_LOG1ssd3");
            Console.WriteLine(bf.Contains("CAR_CAR_LOG1ssd3"));
        }
    }
}