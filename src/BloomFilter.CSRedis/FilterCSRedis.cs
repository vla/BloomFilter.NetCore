using CSRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter.CSRedis
{
    /// <summary>
    /// Bloom Filter Redis Implement
    /// </summary>
    public class FilterCSRedis : Filter
    {
        private readonly CSRedisClient _client;
        private readonly string _redisKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCSRedis"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="client">The <see cref="CSRedisClient"/>.</param>
        /// <param name="redisKey">The redisKey.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterCSRedis(string name, CSRedisClient client, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
            : base(name, expectedElements, errorRate, hashFunction)
        {
            if (string.IsNullOrWhiteSpace(redisKey)) throw new ArgumentException(nameof(redisKey));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _redisKey = redisKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCSRedis"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="client">The <see cref="CSRedisClient"/>.</param>
        /// <param name="redisKey">The redisKey.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="hashes">The hashes.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterCSRedis(string name, CSRedisClient client, string redisKey, int capacity, int hashes, HashFunction hashFunction)
            : base(name, capacity, hashes, hashFunction)
        {
            if (string.IsNullOrWhiteSpace(redisKey)) throw new ArgumentException(nameof(redisKey));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _redisKey = redisKey;
        }

        public override bool Add(byte[] element)
        {
            var positions = ComputeHash(element);
            return SetBit(positions).Any(a => !a);
        }

        public async override Task<bool> AddAsync(byte[] element)
        {
            var positions = ComputeHash(element);
            var results = await SetBitAsync(positions);
            return results.Any(a => !a);
        }

        public override IList<bool> Add(IEnumerable<byte[]> elements)
        {
            var addHashs = new List<int>();
            foreach (var element in elements)
            {
                addHashs.AddRange(ComputeHash(element));
            }

            IList<bool> results = new List<bool>();
            var processResults = SetBit(addHashs.ToArray());
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

        public async override Task<IList<bool>> AddAsync(IEnumerable<byte[]> elements)
        {
            var addHashs = new List<int>();
            foreach (var element in elements)
            {
                addHashs.AddRange(ComputeHash(element));
            }

            IList<bool> results = new List<bool>();
            var processResults = await SetBitAsync(addHashs.ToArray());
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

        public override bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);
            return GetBit(positions).All(a => a);
        }

        public async override Task<bool> ContainsAsync(byte[] element)
        {
            var positions = ComputeHash(element);
            var results = await GetBitAsync(positions);
            return results.All(a => a);
        }

        public override IList<bool> Contains(IEnumerable<byte[]> elements)
        {
            var addHashs = new List<int>();
            foreach (var element in elements)
            {
                addHashs.AddRange(ComputeHash(element));
            }

            IList<bool> results = new List<bool>();

            var processResults = GetBit(addHashs.ToArray());
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

        public async override Task<IList<bool>> ContainsAsync(IEnumerable<byte[]> elements)
        {
            var addHashs = new List<int>();
            foreach (var element in elements)
            {
                addHashs.AddRange(ComputeHash(element));
            }

            IList<bool> results = new List<bool>();

            var processResults = await GetBitAsync(addHashs.ToArray());
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

        public override bool All(IEnumerable<byte[]> elements)
        {
            return Contains(elements).All(e => e);
        }

        public async override Task<bool> AllAsync(IEnumerable<byte[]> elements)
        {
            return (await ContainsAsync(elements)).All(e => e);
        }

        public override void Clear()
        {
            _client.Del(_redisKey);
        }

        public override Task ClearAsync()
        {
            return _client.DelAsync(_redisKey);
        }

        public override void Dispose()
        {
            _client.Dispose();
        }

        private IList<bool> SetBit(int[] positions)
        {
            var results = new bool[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                var n = (uint)positions[i];
                results[i] = _client.SetBit(_redisKey, n, true);
            }

            return results;
        }

        private async Task<IList<bool>> SetBitAsync(int[] positions)
        {
            var results = new bool[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = await _client.SetBitAsync(_redisKey, (uint)positions[i], true);
            }

            return results;
        }

        private IList<bool> GetBit(int[] positions)
        {
            var results = new bool[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = _client.GetBit(_redisKey, (uint)positions[i]);
            }

            return results;
        }

        private async Task<IList<bool>> GetBitAsync(int[] positions)
        {
            var results = new bool[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = await _client.GetBitAsync(_redisKey, (uint)positions[i]);
            }

            return results;
        }
    }
}