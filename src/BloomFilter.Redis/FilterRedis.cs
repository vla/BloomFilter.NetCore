using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter.Redis
{
    /// <summary>
    /// Bloom Filter Redis Implement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="BloomFilter.Filter{T}" />
    public class FilterRedis<T> : Filter<T>
    {
        private readonly IRedisBitOperate _redisBitOperate;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRedis{T}"/> class.
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="name">The name.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterRedis(IRedisBitOperate redisBitOperate, string name, int expectedElements, double errorRate, HashFunction hashFunction)
            : base(expectedElements, errorRate, hashFunction)
        {
            _redisBitOperate = redisBitOperate;
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRedis{T}"/> class.
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="name">The name.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="hashes">The hashes.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterRedis(IRedisBitOperate redisBitOperate, string name, int capacity, int hashes, HashFunction hashFunction)
            : base(capacity, hashes, hashFunction)
        {
            _redisBitOperate = redisBitOperate;
            _name = name;
        }

        /// <summary>
        /// Adds the passed value to the filter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool Add(byte[] element)
        {
            var positions = ComputeHash(element);
            var results = _redisBitOperate.Set(_name, positions, true);
            return results.Any(a => !a);
        }

        public override async Task<bool> AddAsync(byte[] element)
        {
            var positions = ComputeHash(element);
            var results = await _redisBitOperate.SetAsync(_name, positions, true);
            return results.Any(a => !a);
        }

        /// <summary>
        /// Adds the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public override IList<bool> Add(IEnumerable<T> elements)
        {
            var addHashs = new List<int>();
            foreach (var element in elements)
            {
                addHashs.AddRange(ComputeHash(ToBytes(element)));
            }

            IList<bool> results = new List<bool>();

            var processResults = _redisBitOperate.Set(_name, addHashs.ToArray(), true);
            bool wasAdded = false;
            int processed = 0;
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

        public override async Task<IList<bool>> AddAsync(IEnumerable<T> elements)
        {
            var addHashs = new List<int>();
            foreach (var element in elements)
            {
                addHashs.AddRange(ComputeHash(ToBytes(element)));
            }

            IList<bool> results = new List<bool>();

            var processResults = await _redisBitOperate.SetAsync(_name, addHashs.ToArray(), true);
            bool wasAdded = false;
            int processed = 0;
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

        /// <summary>
        /// Removes all elements from the filter
        /// </summary>
        public override void Clear()
        {
            _redisBitOperate.Clear(_name);
        }

        public override Task ClearAsync()
        {
            return _redisBitOperate.ClearAsync(_name);
        }

        /// <summary>
        /// Tests whether an element is present in the filter
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);

            var results = _redisBitOperate.Get(_name, positions);

            return results.All(a => a);
        }

        public override async Task<bool> ContainsAsync(byte[] element)
        {
            var positions = ComputeHash(element);

            var results = await _redisBitOperate.GetAsync(_name, positions);

            return results.All(a => a);
        }

        /// <summary>
        /// Tests whether an elements is present in the filter
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public override IList<bool> Contains(IEnumerable<T> elements)
        {
            var addHashs = new List<int>();
            foreach (var element in elements)
            {
                addHashs.AddRange(ComputeHash(ToBytes(element)));
            }

            IList<bool> results = new List<bool>();

            var processResults = _redisBitOperate.Get(_name, addHashs.ToArray());
            bool isPresent = true;
            int processed = 0;
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

        public override async Task<IList<bool>> ContainsAsync(IEnumerable<T> elements)
        {
            var addHashs = new List<int>();
            foreach (var element in elements)
            {
                addHashs.AddRange(ComputeHash(ToBytes(element)));
            }

            IList<bool> results = new List<bool>();

            var processResults = await _redisBitOperate.GetAsync(_name, addHashs.ToArray());
            bool isPresent = true;
            int processed = 0;
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

        public override void Dispose()
        {
            _redisBitOperate.Dispose();
        }
    }
}