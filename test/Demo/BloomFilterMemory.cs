using BloomFilter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    public class BloomFilterMemory
    {
        [Test("Murmur3KirschMitzenmacher In Memory")]
        public void Default()
        {

            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.SHA1);

            Console.WriteLine("Capacity:" + bf.Capacity);
            Console.WriteLine("Hashes:" + bf.Hashes);
            Console.WriteLine("ErrorRate:" + bf.ErrorRate);
            Console.WriteLine("ExpectedElements:" + bf.ExpectedElements);

            bf.Add("CAR_CAR_LOG1ssd3");
            Console.WriteLine(bf.Contains("CAR_CAR_LOG1ssd3"));

            bf.Clear();
            Console.WriteLine(bf.Contains("CAR_CAR_LOG1ssd3"));

            bf.Add("CAR_CAR_LOG1ssd3");
            Console.WriteLine(bf.Contains("CAR_CAR_LOG1ssd3"));
        }
    }
}
