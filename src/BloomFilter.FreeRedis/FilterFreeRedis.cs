using FreeRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter.FreeRedis
{
    /// <summary>
    /// Bloom Filter Redis Implement
    /// </summary>
    public class FilterFreeRedis : Filter
    {
        private readonly RedisClient _client;
        private readonly string _redisKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterFreeRedis"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="client">The <see cref="RedisClient"/>.</param>
        /// <param name="redisKey">The redisKey.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterFreeRedis(string name, RedisClient client, string redisKey, long expectedElements, double errorRate, HashFunction hashFunction)
            : base(name, expectedElements, errorRate, hashFunction)
        {
            if (string.IsNullOrWhiteSpace(redisKey)) throw new ArgumentException(nameof(redisKey));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _redisKey = redisKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterFreeRedis"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="client">The <see cref="RedisClient"/>.</param>
        /// <param name="redisKey">The redisKey.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="hashes">The hashes.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterFreeRedis(string name, RedisClient client, string redisKey, long capacity, int hashes, HashFunction hashFunction)
            : base(name, capacity, hashes, hashFunction)
        {
            if (string.IsNullOrWhiteSpace(redisKey)) throw new ArgumentException(nameof(redisKey));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _redisKey = redisKey;
        }

        public override bool Add(ReadOnlySpan<byte> data)
        {
            var positions = ComputeHash(data);
            return SetBit(positions).Any(a => !a);
        }

        public override async ValueTask<bool> AddAsync(ReadOnlyMemory<byte> data)
        {
            var positions = ComputeHash(data.Span);
            var results = await SetBitAsync(positions);
            return results.Any(a => !a);
        }

        public override IList<bool> Add(IEnumerable<byte[]> elements)
        {
            var addHashs = new List<long>();
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

        public override async ValueTask<IList<bool>> AddAsync(IEnumerable<byte[]> elements)
        {
            var addHashs = new List<long>();
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

        public override bool Contains(ReadOnlySpan<byte> element)
        {
            var positions = ComputeHash(element);
            return GetBit(positions).All(a => a);
        }

        public override async ValueTask<bool> ContainsAsync(ReadOnlyMemory<byte> element)
        {
            var positions = ComputeHash(element.Span);
            var results = await GetBitAsync(positions);
            return results.All(a => a);
        }

        public override IList<bool> Contains(IEnumerable<byte[]> elements)
        {
            var addHashs = new List<long>();
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

        public override async ValueTask<IList<bool>> ContainsAsync(IEnumerable<byte[]> elements)
        {
            var addHashs = new List<long>();
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

        public override async ValueTask<bool> AllAsync(IEnumerable<byte[]> elements)
        {
            return (await ContainsAsync(elements)).All(e => e);
        }

        public override void Clear()
        {
            _client.Del(_redisKey);
        }

        public override async ValueTask ClearAsync()
        {
            await _client.DelAsync(_redisKey);
        }

        public override void Dispose()
        {
            _client.Dispose();
        }

        private IList<bool> SetBit(long[] positions)
        {
            var results = new bool[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = _client.SetBit(_redisKey, positions[i], true) == 1;
            }

            return results.ToList();
        }

        private async ValueTask<IList<bool>> SetBitAsync(long[] positions)
        {
            var results = new bool[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = await _client.SetBitAsync(_redisKey, positions[i], true) == 1;
            }

            return results.ToList();
        }

        private IList<bool> GetBit(long[] positions)
        {
            var results = new bool[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = _client.GetBit(_redisKey, positions[i]);
            }

            return results.ToList();
        }

        private async ValueTask<IList<bool>> GetBitAsync(long[] positions)
        {
            var results = new bool[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = await _client.GetBitAsync(_redisKey, positions[i]);
            }

            return results.ToList();
        }
    }
}