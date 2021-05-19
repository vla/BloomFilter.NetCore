using BloomFilter;
using BloomFilter.HashAlgorithms;
using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTest
{
    public class HashSpeed
    {
        [Test("HashPerformance")]
        public void Performance()
        {
            HashPerformance(new HashChecksumCrc32());
            HashPerformance(new HashChecksumCrc32u());
            HashPerformance(new HashChecksumAdler32());

            HashPerformance(new HashCryptoSHA1());
            HashPerformance(new HashCryptoSHA256());
            HashPerformance(new HashCryptoSHA384());
            HashPerformance(new HashCryptoSHA512());

            HashPerformance(new LCGWithFNV());
            HashPerformance(new LCGWithFNV1a());
            HashPerformance(new LCGModifiedFNV1());

            HashPerformance(new RNGWithFNV1());
            HashPerformance(new RNGWithFNV1a());
            HashPerformance(new RNGModifiedFNV1());


            HashPerformance(new Murmur2());
            HashPerformance(new Murmur3());
            HashPerformance(new Murmur3KirschMitzenmacher());
        }

        private void HashPerformance(HashFunction hashFunction)
        {
            string name = hashFunction.GetType().Name;
            int m = 1000;
            int k = 10;

            int count = 100000;

            var bf = new FilterMemory(BloomFilterConstValue.DefaultInMemoryName, m, k, hashFunction);

            var array = new List<byte[]>();

            for (int i = 0; i < 1000; i++)
            {
                array.Add(Helper.GenerateBytes(16));
            }

            bf.Add(Helper.GenerateString(20));

            Console.WriteLine(name + ":" + bf);

            int arrayCount = array.Count;

            Helper.Time(name, (n) =>
            {
                bf.ComputeHash(array[n % arrayCount]);
            }, count);


            Helper.TimeWithThread($"{name} {Environment.ProcessorCount} Thread", (t, n) =>
            {
                bf.ComputeHash(array[n % arrayCount]);
            }, Environment.ProcessorCount, count);

            Helper.TimeWithParallel($"{name} Parallel", (n) =>
            {
                bf.ComputeHash(array[n % arrayCount]);
            }, count);

        }

    }
}
