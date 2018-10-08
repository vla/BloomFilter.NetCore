using BloomFilter;
using System;

namespace Demo
{
    public class BloomFilterMemory
    {
        [Test("Murmur3KirschMitzenmacher In Memory")]
        public void Murmur3KirschMitzenmacherTest()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.Murmur3KirschMitzenmacher);
            Sample(bf);
        }

        [Test("Murmur2 In Memory")]
        public void Murmur2Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.Murmur2);
            Sample(bf);
        }

        [Test("Murmur3 In Memory")]
        public void Murmur3Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.Murmur3);
            Sample(bf);
        }

        [Test("Adler32 In Memory")]
        public void Adler32Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.Adler32);
            Sample(bf);
        }

        [Test("CRC32 In Memory")]
        public void CRC32Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.CRC32);
            Sample(bf);
        }

        [Test("MD5 In Memory")]
        public void MD5Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.MD5);
            Sample(bf);
        }

        [Test("SHA1 In Memory")]
        public void SHA1Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.SHA1);
            Sample(bf);
        }

        [Test("SHA256 In Memory")]
        public void SHA256Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.SHA256);
            Sample(bf);
        }

        [Test("SHA512 In Memory")]
        public void SHA512Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.SHA512);
            Sample(bf);
        }

        [Test("RNGWithFNV1 In Memory")]
        public void RNGWithFNV1Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.RNGWithFNV1);
            Sample(bf);
        }

        [Test("LCGWithFNV1 In Memory")]
        public void LCGWithFNV1Test()
        {
            var bf = FilterBuilder.Build<string>(10000000, 0.01, HashMethod.LCGWithFNV1);
            Sample(bf);
        }

        private void Sample(Filter<string> bf)
        {
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