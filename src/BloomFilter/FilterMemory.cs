using System;
using System.Collections;

namespace BloomFilter
{
    public class FilterMemory<T> : Filter<T>
    {
        private BitArray _hashBits;

        public FilterMemory(int expectedElements, double errorRate, HashFunction hashFunction)
            : base(expectedElements, errorRate, hashFunction)
        {
            _hashBits = new BitArray(Capacity);
        }

        public FilterMemory(int size, int hashes, HashFunction hashFunction)
            : base(size, hashes, hashFunction)
        {
            _hashBits = new BitArray(Capacity);
        }

        public override bool Add(byte[] element)
        {
            bool added = false;

            foreach (int position in ComputeHash(element))
            {
                if (!_hashBits.Get(position))
                {
                    added = true;
                    _hashBits.Set(position, true);
                }
            }

            return added;
        }

        public override bool Contains(byte[] element)
        {
            foreach (int position in ComputeHash(element))
            {
                if (!_hashBits.Get(position))
                    return false;
            }
            return true;
        }

        public override void Clear()
        {
            _hashBits.SetAll(false);
        }
    }
}