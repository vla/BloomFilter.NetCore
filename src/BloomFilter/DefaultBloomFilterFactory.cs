using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BloomFilter
{
    public class DefaultBloomFilterFactory : IBloomFilterFactory
    {
        private readonly IEnumerable<IBloomFilter> _bloomFilters;

        public DefaultBloomFilterFactory(IEnumerable<IBloomFilter> bloomFilters)
        {
            _bloomFilters = bloomFilters;
        }

        public IBloomFilter Get(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var filter = _bloomFilters.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (filter == null) throw new ArgumentException("can not find a match BloomFilter");

            return filter;
        }

        public bool TryGet(string name,
#if NET5_0_OR_GREATER
            [MaybeNullWhen(false)]
#endif
            out IBloomFilter bloomFilter)
        {
            bloomFilter = _bloomFilters.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            var dict = new Dictionary<string, string>();
            return bloomFilter != null;
        }
    }
}