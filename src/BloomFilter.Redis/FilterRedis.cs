using System.Collections.Generic;
using System.Linq;

namespace BloomFilter.Redis
{
    public class FilterRedis<T> : Filter<T>
    {
        private readonly IRedisBitOperate _redisBitOperate;
        private readonly string _name;

        public FilterRedis(IRedisBitOperate redisBitOperate, string name, int expectedElements, double errorRate, HashFunction hashFunction)
            : base(expectedElements, errorRate, hashFunction)
        {
            _redisBitOperate = redisBitOperate;
            _name = name;
        }

        public FilterRedis(IRedisBitOperate redisBitOperate, string name, int capacity, int hashes, HashFunction hashFunction)
            : base(capacity, hashes, hashFunction)
        {
            _redisBitOperate = redisBitOperate;
            _name = name;
        }

        public override bool Add(byte[] element)
        {
            var positions = ComputeHash(element);
            var results = _redisBitOperate.Set(_name, positions, true);
            return results.Any(a => !a);
        }

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

        public override void Clear()
        {
            _redisBitOperate.Clear(_name);
        }

        public override bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);

            var results = _redisBitOperate.Get(_name, positions);

            return results.All(a => a);
        }

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
    }
}