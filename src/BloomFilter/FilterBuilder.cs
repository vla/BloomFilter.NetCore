using BloomFilter.HashAlgorithms;
using System.Collections.Generic;

namespace BloomFilter
{
    /// <summary>
    /// BloomFilter Builder
    /// </summary>
    public partial class FilterBuilder
    {
        /// <summary>
        /// The hash functions
        /// </summary>
        protected readonly static IReadOnlyDictionary<HashMethod, HashFunction> HashFunctions = new Dictionary<HashMethod, HashFunction>
        {
            { HashMethod.LCGWithFNV1,new LCGWithFNV() },
            { HashMethod.LCGWithFNV1a,new LCGWithFNV1a() },
            { HashMethod.LCGModifiedFNV1,new LCGModifiedFNV1() },

            { HashMethod.RNGWithFNV1,new RNGWithFNV1() },
            { HashMethod.RNGWithFNV1a,new RNGWithFNV1a() },
            { HashMethod.RNGModifiedFNV1,new RNGModifiedFNV1() },

            { HashMethod.CRC32,new HashChecksumCrc32() },
            { HashMethod.CRC32u,new HashChecksumCrc32u() },
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

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements)
        {
            return Build<T>(expectedElements, 0.01);
        }

        /// <summary>
        ///Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, HashMethod hashMethod)
        {
            return Build<T>(expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, HashFunction hashFunction)
        {
            return Build<T>(expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, double errorRate)
        {
            return Build<T>(expectedElements, errorRate, HashFunctions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterMemory<T>(expectedElements, errorRate, HashFunctions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterMemory<T>(expectedElements, errorRate, hashFunction);
        }
    }

    /// <summary>
    /// Hash Methods
    /// </summary>
    public enum HashMethod
    {
        LCGWithFNV1,
        LCGWithFNV1a,
        LCGModifiedFNV1,
        RNGWithFNV1,
        RNGWithFNV1a,
        RNGModifiedFNV1,
        CRC32,
        CRC32u,
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