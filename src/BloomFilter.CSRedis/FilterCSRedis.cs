using CSRedis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter.CSRedis;

/// <summary>
/// Bloom Filter Redis Implementation using CSRedisCore
/// </summary>
public class FilterCSRedis : FilterRedisBase
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
    public FilterCSRedis(string name, CSRedisClient client, string redisKey, long expectedElements, double errorRate, HashFunction hashFunction)
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
    public FilterCSRedis(string name, CSRedisClient client, string redisKey, long capacity, int hashes, HashFunction hashFunction)
        : base(name, capacity, hashes, hashFunction)
    {
        if (string.IsNullOrWhiteSpace(redisKey)) throw new ArgumentException(nameof(redisKey));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _redisKey = redisKey;
    }

    // Implement abstract methods - CSRedis specific operations

    protected override bool SetBit(long position)
        => _client.SetBit(_redisKey, (uint)position, true);

    protected override bool GetBit(long position)
        => _client.GetBit(_redisKey, (uint)position);

    protected override async Task<bool> SetBitAsync(long position)
        => await _client.SetBitAsync(_redisKey, (uint)position, true);

    protected override async Task<bool> GetBitAsync(long position)
        => await _client.GetBitAsync(_redisKey, (uint)position);

    protected override bool[] SetBits(long[] positions)
    {
        // Use pipeline for batch execution
        using var pipe = _client.StartPipe();

        for (int i = 0; i < positions.Length; i++)
        {
            pipe.SetBit(_redisKey, (uint)positions[i], true);
        }

        var pipeResults = pipe.EndPipe();
        var results = new bool[positions.Length];

        for (int i = 0; i < pipeResults.Length; i++)
        {
            results[i] = Convert.ToBoolean(pipeResults[i]);
        }

        return results;
    }

    protected override bool[] GetBits(long[] positions)
    {
        // Use pipeline for batch execution
        using var pipe = _client.StartPipe();

        for (int i = 0; i < positions.Length; i++)
        {
            pipe.GetBit(_redisKey, (uint)positions[i]);
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
        var tasks = new Task<bool>[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            tasks[i] = _client.SetBitAsync(_redisKey, (uint)positions[i], true);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        var results = new bool[tasks.Length];
        for (int i = 0; i < tasks.Length; i++)
        {
            results[i] = tasks[i].Result;
        }

        return results;
    }

    protected override async Task<bool[]> GetBitsAsync(long[] positions)
    {
        // Execute all operations in parallel
        var tasks = new Task<bool>[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            tasks[i] = _client.GetBitAsync(_redisKey, (uint)positions[i]);
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
