using FreeRedis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter.FreeRedis;

/// <summary>
/// Bloom Filter Redis Implementation using FreeRedis
/// </summary>
public class FilterFreeRedis : FilterRedisBase
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

    // Implement abstract methods - FreeRedis specific operations

    protected override bool SetBit(long position)
        => _client.SetBit(_redisKey, position, true) == 1;

    protected override bool GetBit(long position)
        => _client.GetBit(_redisKey, position);

    protected override async Task<bool> SetBitAsync(long position)
        => await _client.SetBitAsync(_redisKey, position, true) == 1;

    protected override async Task<bool> GetBitAsync(long position)
        => await _client.GetBitAsync(_redisKey, position);

    protected override bool[] SetBits(long[] positions)
    {
        // Use pipeline for batch execution
        using var pipe = _client.StartPipe();

        for (int i = 0; i < positions.Length; i++)
        {
            pipe.SetBit(_redisKey, positions[i], true);
        }

        var pipeResults = pipe.EndPipe();
        var results = new bool[positions.Length];

        for (int i = 0; i < pipeResults.Length; i++)
        {
            results[i] = Convert.ToInt64(pipeResults[i]) == 1;
        }

        return results;
    }

    protected override bool[] GetBits(long[] positions)
    {
        // Use pipeline for batch execution
        using var pipe = _client.StartPipe();

        for (int i = 0; i < positions.Length; i++)
        {
            pipe.GetBit(_redisKey, positions[i]);
        }

        var pipeResults = pipe.EndPipe();
        var results = new bool[positions.Length];

        for (int i = 0; i < pipeResults.Length; i++)
        {
            results[i] = Convert.ToBoolean(pipeResults[i]);
        }

        return results;
    }

    protected override async Task<bool[]> SetBitsAsync(long[] positions)
    {
        // Execute all operations in parallel
        var tasks = new Task<long>[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            tasks[i] = _client.SetBitAsync(_redisKey, positions[i], true);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        var results = new bool[tasks.Length];
        for (int i = 0; i < tasks.Length; i++)
        {
            results[i] = tasks[i].Result == 1;
        }

        return results;
    }

    protected override async Task<bool[]> GetBitsAsync(long[] positions)
    {
        // Execute all operations in parallel
        var tasks = new Task<bool>[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            tasks[i] = _client.GetBitAsync(_redisKey, positions[i]);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        var results = new bool[tasks.Length];
        for (int i = 0; i < tasks.Length; i++)
        {
            results[i] = tasks[i].Result;
        }

        return results;
    }

    protected override void ClearBits()
        => _client.Del(_redisKey);

    protected override async Task ClearBitsAsync()
        => await _client.DelAsync(_redisKey);

    // Single element operations

    public override bool Add(ReadOnlySpan<byte> data)
    {
        var positions = ComputeHash(data);
        return SetBits(positions).Any(a => !a);
    }

    public override async ValueTask<bool> AddAsync(ReadOnlyMemory<byte> data)
    {
        var positions = ComputeHash(data.Span);
        var results = await SetBitsAsync(positions);
        return results.Any(a => !a);
    }

    public override bool Contains(ReadOnlySpan<byte> element)
    {
        var positions = ComputeHash(element);
        return GetBits(positions).All(a => a);
    }

    public override async ValueTask<bool> ContainsAsync(ReadOnlyMemory<byte> element)
    {
        var positions = ComputeHash(element.Span);
        var results = await GetBitsAsync(positions);
        return results.All(a => a);
    }

    public override void Dispose()
    {
        _client.Dispose();
    }
}