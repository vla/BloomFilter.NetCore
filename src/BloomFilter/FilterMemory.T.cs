using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter
{
    /// <summary>
    /// Bloom Filter In Mempory Implement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Filter{T}" />
    [Obsolete("Use non-generic FilterMemory")]
    public class FilterMemory<T> : Filter<T>
    {
        private readonly BitArray _hashBits;

        private readonly object sync = new();

        private readonly static Task Empty = Task.FromResult(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMemory{T}"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterMemory(string name, int expectedElements, double errorRate, HashFunction hashFunction)
            : base(name, expectedElements, errorRate, hashFunction)
        {
            _hashBits = new BitArray(Capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMemory{T}"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size">The size.</param>
        /// <param name="hashes">The hashes.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterMemory(string name, int size, int hashes, HashFunction hashFunction)
            : base(name, size, hashes, hashFunction)
        {
            _hashBits = new BitArray(Capacity);
        }

        /// <summary>
        /// Adds the passed value to the filter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool Add(byte[] element)
        {
            bool added = false;
            var positions = ComputeHash(element);
            lock (sync)
            {
                foreach (int position in positions)
                {
                    if (!_hashBits.Get(position))
                    {
                        added = true;
                        _hashBits.Set(position, true);
                    }
                }
            }
            return added;
        }

        /// <summary>
        /// Adds the passed value to the filter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override Task<bool> AddAsync(byte[] element)
        {
            return Task.FromResult(Add(element));
        }

        public override IList<bool> Add(IEnumerable<byte[]> elements)
        {
            var hashes = new List<int>();
            foreach (var element in elements)
            {
                hashes.AddRange(ComputeHash(element));
            }

            var processResults = new bool[hashes.Count];
            lock (sync)
            {
                for (var i = 0; i < hashes.Count; i++)
                {
                    if (!_hashBits.Get(hashes[i]))
                    {
                        _hashBits.Set(hashes[i], true);
                        processResults[i] = false;
                    }
                    else
                    {
                        processResults[i] = true;
                    }
                }
            }

            IList<bool> results = new List<bool>();
            bool wasAdded = false;
            int processed = 0;

            //For each value check, if all bits in ranges of hashes bits are set
            foreach (var item in processResults)
            {
                if (!item) wasAdded = true;
                if ((processed + 1) % Hashes == 0)
                {
                    results.Add(wasAdded);
                    wasAdded = false;
                }
                processed++;
            }

            return results;
        }

        public override Task<IList<bool>> AddAsync(IEnumerable<byte[]> elements)
        {
            return Task.FromResult(Add(elements));
        }

        /// <summary>
        /// Tests whether an element is present in the filter
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);
            lock (sync)
            {
                foreach (int position in positions)
                {
                    if (!_hashBits.Get(position))
                        return false;
                }
            }
            return true;
        }

        public override Task<bool> ContainsAsync(byte[] element)
        {
            return Task.FromResult(Contains(element));
        }

        public override IList<bool> Contains(IEnumerable<byte[]> elements)
        {
            var hashes = new List<int>();
            foreach (var element in elements)
            {
                hashes.AddRange(ComputeHash(element));
            }

            var processResults = new bool[hashes.Count];
            lock (sync)
            {
                for (var i = 0; i < hashes.Count; i++)
                {
                    processResults[i] = _hashBits.Get(hashes[i]);
                }
            }

            IList<bool> results = new List<bool>();
            bool isPresent = true;
            int processed = 0;

            //For each value check, if all bits in ranges of hashes bits are set
            foreach (var item in processResults)
            {
                if (!item) isPresent = false;
                if ((processed + 1) % Hashes == 0)
                {
                    results.Add(isPresent);
                    isPresent = true;
                }
                processed++;
            }

            return results;
        }

        public override Task<IList<bool>> ContainsAsync(IEnumerable<byte[]> elements)
        {
            return Task.FromResult(Contains(elements));
        }

        public override bool All(IEnumerable<byte[]> elements)
        {
            return Contains(elements).All(e => e);
        }

        public override Task<bool> AllAsync(IEnumerable<byte[]> elements)
        {
            return Task.FromResult(All(elements));
        }

        /// <summary>
        /// Removes all elements from the filter
        /// </summary>
        public override void Clear()
        {
            lock (sync)
            {
                _hashBits.SetAll(false);
            }
        }

        public override Task ClearAsync()
        {
            Clear();
            return Empty;
        }

        public override void Dispose()
        {
        }
    }
}