using System;

namespace BloomFilter.HashAlgorithms
{
    /// <summary>
    /// Implements an Linear Congruential Generator With  FNV1a hash algorithm.
    /// </summary>
    public class LCGWithFNV1a : LCGWithFNV
    {
        public override uint Hash(byte[] data)
        {
            int end = data.Length;
            uint hash = Init;
            for (int i = 0; i < end; i++)
            {
                hash ^= data[i];
                hash *= Prime;
            }
            return hash;
        }
    }

    /// <summary>
    /// Implements an Linear Congruential Generator With  modified FNV hash. Provides better distribution than FNV1 but it's only 32 bit long.
    /// </summary>
    public class LCGModifiedFNV1 : LCGWithFNV1a
    {
        public override uint Hash(byte[] data)
        {
            var hash = base.Hash(data);
            hash += hash << 13;
            hash ^= hash >> 7;
            hash += hash << 3;
            hash ^= hash >> 17;
            hash += hash << 5;
            return hash;
        }
    }

    /// <summary>
    /// Implements an Linear Congruential Generator With FNV1 hash algorithm.
    /// </summary>
    public class LCGWithFNV : HashFunction
    {
        protected const uint Prime = 16777619;
        protected const uint Init = 2166136261;

        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            long multiplier = 0x5DEECE66DL;
            long addend = 0xBL;
            long mask = (1L << 48) - 1;

            // Generate int from byte Array using the FNV hash
            int reduced = Math.Abs((int)Hash(data));
            // Make number positive
            // Handle the special case: smallest negative number is itself as the
            // absolute value
            if (reduced == int.MaxValue)
                reduced = 42;

            // Calculate hashes numbers iteratively
            int[] positions = new int[k];
            long seed = reduced;
            for (int i = 0; i < k; i++)
            {
                //http://en.wikipedia.org/wiki/Linear_congruential_generator
                // LCG formula: x_i+1 = (multiplier * x_i + addend) mod mask
                seed = (seed * multiplier + addend) & mask;
                positions[i] = (int)(RightMove(seed, (48 - 30))) % m;
            }
            return positions;
        }

        public virtual uint Hash(byte[] data)
        {
            int end = data.Length;
            uint hash = Init;
            for (int i = 0; i < end; i++)
            {
                hash *= Prime;
                hash ^= data[i];
            }
            return hash;
        }
    }
}