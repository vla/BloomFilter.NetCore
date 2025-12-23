using System;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter.Redis;

/// <summary>
/// Bloom Filter Redis Implementation using StackExchange.Redis
/// </summary>
public class FilterRedis : FilterRedisBase
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

    // Implement abstract methods - StackExchange.Redis specific operations

    protected override bool SetBit(long position)
        => _redisBitOperate.Set(_redisKey, position, true);

    protected override bool GetBit(long position)
        => _redisBitOperate.Get(_redisKey, position);

    protected override async Task<bool> SetBitAsync(long position)
        => await _redisBitOperate.SetAsync(_redisKey, position, true);

    protected override async Task<bool> GetBitAsync(long position)
        => await _redisBitOperate.GetAsync(_redisKey, position);

    protected override bool[] SetBits(long[] positions)
        => _redisBitOperate.Set(_redisKey, positions, true);

    protected override bool[] GetBits(long[] positions)
        => _redisBitOperate.Get(_redisKey, positions);

    protected override async Task<bool[]> SetBitsAsync(long[] positions)
        => await _redisBitOperate.SetAsync(_redisKey, positions, true);

    protected override async Task<bool[]> GetBitsAsync(long[] positions)
        => await _redisBitOperate.GetAsync(_redisKey, positions);

    protected override void ClearBits()
        => _redisBitOperate.Clear(_redisKey);

    protected override async Task ClearBitsAsync()
        => await _redisBitOperate.ClearAsync(_redisKey);

    // Single element operations (not part of base class - keep existing implementation)

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

    public override void Dispose()
    {
        _redisBitOperate.Dispose();
    }
}
