using System;
using System.Collections;
using System.Collections.Generic;
using BloomFilter.HashAlgorithms;

namespace BloomFilter
{
    /// <summary>
    /// An implemented to provide custom hash functions.
    /// </summary>
    public abstract class HashFunction
    {
        /// <summary>
        /// The hash functions
        /// </summary>
        public readonly static IReadOnlyDictionary<HashMethod, HashFunction> Functions = new Dictionary<HashMethod, HashFunction>
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

            { HashMethod.SHA1,new HashCryptoSHA1() },
            { HashMethod.SHA256,new HashCryptoSHA256() },
            { HashMethod.SHA384,new HashCryptoSHA384() },
            { HashMethod.SHA512,new HashCryptoSHA512() },
        };

        private const int IntMax = 2147483647;

        /// <summary>
        /// Hashes the specified value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="m">integer output range.</param>
        /// <param name="k">number of hashes to be computed.</param>
        /// <returns>
        /// int array of hashes hash values
        /// </returns>
        public abstract int[] ComputeHash(byte[] data, int m, int k);

        /// <summary>
        /// Perform rejection sampling on a 32-bit,
        ///https://en.wikipedia.org/wiki/Rejection_sampling
        /// </summary>
        /// <param name="random">The random.</param>
        /// <param name="m">integer output range.</param>
        /// <returns></returns>
        protected int Rejection(uint random, int m)
        {
            return Rejection((int)random, m);
        }

        /// <summary>
        /// Perform rejection sampling on a 32-bit,
        /// https://en.wikipedia.org/wiki/Rejection_sampling
        /// </summary>
        /// <param name="random">The random.</param>
        /// <param name="m">integer output range.</param>
        /// <returns></returns>
        protected int Rejection(int random, int m)
        {
            random = Math.Abs(random);
            if (random > (IntMax - IntMax % m) || random == IntMax)
                return -1;
            return random % m;
        }

        public static uint NumberOfTrailingZeros(uint i)
        {
            // HD, Figure 5-14
            uint y;
            if (i == 0) return 32;
            uint n = 31;
            y = i << 16; if (y != 0) { n -= 16; i = y; }
            y = i << 8; if (y != 0) { n -= 8; i = y; }
            y = i << 4; if (y != 0) { n -= 4; i = y; }
            y = i << 2; if (y != 0) { n -= 2; i = y; }
            return n - ((i << 1) >> 31);
        }

        public static uint NumberOfLeadingZeros(uint i)
        {
            // HD, Figure 5-6
            if (i == 0)
                return 32;
            uint n = 1;
            if (i >> 16 == 0) { n += 16; i <<= 16; }
            if (i >> 24 == 0) { n += 8; i <<= 8; }
            if (i >> 28 == 0) { n += 4; i <<= 4; }
            if (i >> 30 == 0) { n += 2; i <<= 2; }
            n -= i >> 31;
            return n;
        }

        public static uint RotateLeft(uint original, int bits)
        {
            return (original << bits) | (original >> (32 - bits));
        }

        public static uint RotateRight(uint original, int bits)
        {
            return (original >> bits) | (original << (32 - bits));
        }

        public static ulong RotateLeft(ulong original, int bits)
        {
            return (original << bits) | (original >> (64 - bits));
        }

        public static ulong RotateRight(ulong original, int bits)
        {
            return (original >> bits) | (original << (64 - bits));
        }

        public static int BitToIntOne(BitArray bit, int from, int to)
        {
            const int size = 32;
            int len = to - from;
            int bitCount = bit.Count;
            int result = 0;

            for (int i = 0; i < len && i < bitCount && i < size; i++)
            {
                result = bit[i + from] ? result + (1 << i) : result;
            }

            return result;
        }

        public static int RightMove(int value, int pos)
        {
            if (pos != 0)
            {
                var mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }

            return value;
        }

        public static long RightMove(long value, int pos)
        {
            if (pos != 0)
            {
                var mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }

            return value;
        }

        public static uint RightMove(uint value, int pos)
        {
            if (pos != 0)
            {
                uint mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }

            return value;
        }
    }
}