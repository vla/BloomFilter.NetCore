using BloomFilter.HashAlgorithms;
using System;
using System.Collections.Generic;
using System.Text;

namespace BloomFilter
{
    public partial class FilterBuilder
    {
        private static Dictionary<HashMethod, HashFunction> HashFunctions;

        static FilterBuilder()
        {
            HashFunctions = new Dictionary<HashMethod, HashFunction>
            {
                { HashMethod.LCGWithFNV1,new LCGWithFNV() },
                { HashMethod.LCGWithFNV1a,new LCGWithFNV1a() },
                { HashMethod.LCGModifiedFNV1,new LCGModifiedFNV1() },

                { HashMethod.RNGWithFNV1,new RNGWithFNV1() },
                { HashMethod.RNGWithFNV1a,new RNGWithFNV1a() },
                { HashMethod.RNGModifiedFNV1,new RNGModifiedFNV1() },

                { HashMethod.CRC32,new HashChecksumCrc32() },
                { HashMethod.Adler32,new HashChecksumAdler32() },

                { HashMethod.Murmur2,new Murmur2() },
                { HashMethod.Murmur3,new Murmur3() },
                { HashMethod.Murmur3KirschMitzenmacher,new Murmur3KirschMitzenmacher() },

                { HashMethod.MD5,new HashCryptoMD5() },
                { HashMethod.SHA1,new HashCryptoSHA1() },
                { HashMethod.SHA256,new HashCryptoSHA256() },
                { HashMethod.SHA384,new HashCryptoSHA384() },
                { HashMethod.SHA512,new HashCryptoSHA512() },
            };
        }

        public static Filter<T> Build<T>(int expectedElements)
        {
            return Build<T>(expectedElements, 0.01);
        }

        public static Filter<T> Build<T>(int expectedElements, HashMethod hashMethod)
        {
            return Build<T>(expectedElements, 0.01, hashMethod);
        }

        public static Filter<T> Build<T>(int expectedElements, HashFunction hashFunction)
        {
            return Build<T>(expectedElements, 0.01, hashFunction);
        }

        public static Filter<T> Build<T>(int expectedElements, double errorRate)
        {
            return Build<T>(expectedElements, errorRate, HashFunctions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        public static Filter<T> Build<T>(int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterMemory<T>(expectedElements, errorRate, HashFunctions[hashMethod]);
        }

        public static Filter<T> Build<T>(int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterMemory<T>(expectedElements, errorRate, hashFunction);
        }
    }

    public enum HashMethod
    {
        LCGWithFNV1,
        LCGWithFNV1a,
        LCGModifiedFNV1,
        RNGWithFNV1,
        RNGWithFNV1a,
        RNGModifiedFNV1,
        CRC32,
        Adler32,
        Murmur2,
        Murmur3,
        Murmur3KirschMitzenmacher,
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }
}