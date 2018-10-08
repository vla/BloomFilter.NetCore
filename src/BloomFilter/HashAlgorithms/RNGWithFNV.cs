using System;

namespace BloomFilter.HashAlgorithms
{
    /// <summary>
    /// Implements an Random With FNV1a hash algorithm.
    /// </summary>
    public class RNGWithFNV1a : RNGWithFNV1
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
    /// Implements an Random With modified FNV hash. Provides better distribution than FNV1 but it's only 32 bit long.
    /// </summary>
    public class RNGModifiedFNV1 : RNGWithFNV1a
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
    /// Implements an Random With FNV1 hash algorithm.
    /// </summary>
    public class RNGWithFNV1 : HashFunction
    {
        protected const uint Prime = 16777619;
        protected const uint Init = 2166136261;

        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            int[] positions = new int[k];

            Random r = new Random((int)Hash(data));

            for (int i = 0; i < k; i++)
            {
                positions[i] = r.Next(m);
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