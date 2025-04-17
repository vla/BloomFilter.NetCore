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
            HashPerformance(new Crc32());
            HashPerformance(new Crc64());
            HashPerformance(new Adler32());

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


            HashPerformance(new Murmur32BitsX86());
            HashPerformance(new Murmur128BitsX64());
            HashPerformance(new Murmur128BitsX86());

            HashPerformance(new XXHash32());
            HashPerformance(new XXHash64());
            HashPerformance(new XXHash3());
            HashPerformance(new XXHash128());
        }

        private void HashPerformance(HashFunction hashFunction)
        {
            string name = hashFunction.GetType().Name;
            long m = 1000;
            int k = 10;

            int count = 100000;

            var bf = new FilterMemory(BloomFilterConstValue.DefaultInMemoryName, m, k, hashFunction, new DefaultFilterMemorySerializer());

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
