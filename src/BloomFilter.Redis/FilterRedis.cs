using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter.Redis;

/// <summary>
/// Bloom Filter Redis Implement
/// </summary>
public class FilterRedis : Filter
{
    private readonly IRedisBitOperate _redisBitOperate;
    private readonly string _redisKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterRedis"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="redisBitOperate">The redis bit operate.</param>
    /// <param name="redisKey">The redisKey.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    public FilterRedis(string name, IRedisBitOperate redisBitOperate, string redisKey, long expectedElements, double errorRate, HashFunction hashFunction)
        : base(name, expectedElements, errorRate, hashFunction)
    {
        if (string.IsNullOrWhiteSpace(redisKey)) throw new ArgumentException(nameof(redisKey));
        _redisBitOperate = redisBitOperate ?? throw new ArgumentNullException(nameof(redisBitOperate));
        _redisKey = redisKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterRedis"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="redisBitOperate">The redis bit operate.</param>
    /// <param name="redisKey">The redisKey.</param>
    /// <param name="capacity">The capacity.</param>
    /// <param name="hashes">The hashes.</param>
    /// <param name="hashFunction">The hash function.</param>
    public FilterRedis(string name, IRedisBitOperate redisBitOperate, string redisKey, long capacity, int hashes, HashFunction hashFunction)
        : base(name, capacity, hashes, hashFunction)
    {
        if (string.IsNullOrWhiteSpace(redisKey)) throw new ArgumentException(nameof(redisKey));
        _redisBitOperate = redisBitOperate ?? throw new ArgumentNullException(nameof(redisBitOperate));
        _redisKey = redisKey;
    }

    /// <summary>
    /// Adds the passed value to the filter.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override bool Add(ReadOnlySpan<byte> data)
    {
        var positions = ComputeHash(data);
        var results = _redisBitOperate.Set(_redisKey, positions, true);
        return results.Any(a => !a);
    }

    public override async ValueTask<bool> AddAsync(ReadOnlyMemory<byte> data)
    {
        var positions = ComputeHash(data.Span);
        var results = await _redisBitOperate.SetAsync(_redisKey, positions, true);
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

        var processResults = _redisBitOperate.Set(_redisKey, addHashs.ToArray(), true);
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

        var processResults = await _redisBitOperate.SetAsync(_redisKey, addHashs.ToArray(), true);
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
    /// Tests whether an element is present in the filter
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public override bool Contains(ReadOnlySpan<byte> element)
    {
        var positions = ComputeHash(element);

        var results = _redisBitOperate.Get(_redisKey, positions);

        return results.All(a => a);
    }

    public override async ValueTask<bool> ContainsAsync(ReadOnlyMemory<byte> element)
    {
        var positions = ComputeHash(element.Span);

        var results = await _redisBitOperate.GetAsync(_redisKey, positions);

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

        var processResults = _redisBitOperate.Get(_redisKey, addHashs.ToArray());
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

        var processResults = await _redisBitOperate.GetAsync(_redisKey, addHashs.ToArray());
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

    /// <summary>
    /// Removes all elements from the filter
    /// </summary>
    public override void Clear()
    {
        _redisBitOperate.Clear(_redisKey);
    }

    public override async ValueTask ClearAsync()
    {
        await _redisBitOperate.ClearAsync(_redisKey);
    }

    public override void Dispose()
    {
        _redisBitOperate.Dispose();
    }
}