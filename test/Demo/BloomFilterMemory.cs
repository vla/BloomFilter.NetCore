using BloomFilter;
using System;

namespace Demo
{
    public class BloomFilterMemory
    {
        [Test("Sample In Memory")]
        public void SampleInMem()
        {

            var names = Enum.GetNames(typeof(HashMethod));
            foreach (var name in names)
            {
                if (Enum.TryParse<HashMethod>(name, out var hm))
                {
                    var bf = FilterBuilder.Build(10000000, 0.01, hm);
                    Sample(bf);
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