using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter.EasyCaching;

/// <summary>
/// Bloom Filter Redis Implementation using EasyCaching
/// </summary>
public class FilterEasyCachingRedis : FilterRedisBase
{
    private readonly string _redisKey;
    private readonly IRedisCachingProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterEasyCachingRedis"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="provider">The <see cref="IRedisCachingProvider"/>.</param>
    /// <param name="redisKey">The redisKey.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    public FilterEasyCachingRedis(string name, IRedisCachingProvider provider, string redisKey, long expectedElements, double errorRate, HashFunction hashFunction)
        : base(name, expectedElements, errorRate, hashFunction)
    {
        if (string.IsNullOrWhiteSpace(redisKey)) throw new ArgumentException(nameof(redisKey));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _redisKey = redisKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterEasyCachingRedis"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="provider">The <see cref="IRedisCachingProvider"/>.</param>
    /// <param name="redisKey">The redisKey.</param>
    /// <param name="capacity">The capacity.</param>
    /// <param name="hashes">The hashes.</param>
    /// <param name="hashFunction">The hash function.</param>
    public FilterEasyCachingRedis(string name, IRedisCachingProvider provider, string redisKey, long capacity, int hashes, HashFunction hashFunction)
        : base(name, capacity, hashes, hashFunction)
    {
        if (string.IsNullOrWhiteSpace(redisKey)) throw new ArgumentException(nameof(redisKey));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _redisKey = redisKey;
    }

    // Implement abstract methods - EasyCaching specific operations

    protected override bool SetBit(long position)
    {
        var result = _provider.Eval("return redis.call('SETBIT', KEYS[1], ARGV[1], 1)",
            _redisKey, new object[] { position }.ToList());
        return result.ToString() == "1";
    }

    protected override bool GetBit(long position)
    {
        var result = _provider.Eval("return redis.call('GETBIT', KEYS[1], ARGV[1])",
            _redisKey, new object[] { position }.ToList());
        return result.ToString() == "1";
    }

    protected override async Task<bool> SetBitAsync(long position)
    {
        var result = await _provider.EvalAsync("return redis.call('SETBIT', KEYS[1], ARGV[1], 1)",
            _redisKey, new object[] { position }.ToList()).ConfigureAwait(false);
        return result.ToString() == "1";
    }

    protected override async Task<bool> GetBitAsync(long position)
    {
        var result = await _provider.EvalAsync("return redis.call('GETBIT', KEYS[1], ARGV[1])",
            _redisKey, new object[] { position }.ToList()).ConfigureAwait(false);
        return result.ToString() == "1";
    }

    protected override bool[] SetBits(long[] positions)
    {
        // Use Lua script for batch execution
        const string luaScript = @"
local results = {}
for i, pos in ipairs(ARGV) do
    results[i] = redis.call('SETBIT', KEYS[1], pos, 1)
end
return results";

        var result = _provider.Eval(luaScript, _redisKey, positions.Cast<object>().ToList());

        if (result is object[] resultArray)
        {
            var results = new bool[resultArray.Length];
            for (int i = 0; i < resultArray.Length; i++)
            {
                results[i] = resultArray[i].ToString() == "1";
            }
            return results;
        }

        // Fallback: if Lua script fails, use sequential execution
        var fallbackResults = new bool[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            var singleResult = _provider.Eval("return redis.call('SETBIT', KEYS[1], ARGV[1], 1)",
                _redisKey, new object[] { positions[i] }.ToList());
            fallbackResults[i] = singleResult.ToString() == "1";
        }
        return fallbackResults;
    }

    protected override bool[] GetBits(long[] positions)
    {
        // Use Lua script for batch execution
        const string luaScript = @"
local results = {}
for i, pos in ipairs(ARGV) do
    results[i] = redis.call('GETBIT', KEYS[1], pos)
end
return results";

        var result = _provider.Eval(luaScript, _redisKey, positions.Cast<object>().ToList());

        if (result is object[] resultArray)
        {
            var results = new bool[resultArray.Length];
            for (int i = 0; i < resultArray.Length; i++)
            {
                results[i] = resultArray[i].ToString() == "1";
            }
            return results;
        }

        // Fallback: if Lua script fails, use sequential execution
        var fallbackResults = new bool[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            var singleResult = _provider.Eval("return redis.call('GETBIT', KEYS[1], ARGV[1])",
                _redisKey, new object[] { positions[i] }.ToList());
            fallbackResults[i] = singleResult.ToString() == "1";
        }
        return fallbackResults;
    }

    protected override async Task<bool[]> SetBitsAsync(long[] positions)
    {
        // Use Lua script for batch execution
        const string luaScript = @"
local results = {}
for i, pos in ipairs(ARGV) do
    results[i] = redis.call('SETBIT', KEYS[1], pos, 1)
end
return results";

        var result = await _provider.EvalAsync(luaScript, _redisKey, positions.Cast<object>().ToList()).ConfigureAwait(false);

        if (result is object[] resultArray)
        {
            var results = new bool[resultArray.Length];
            for (int i = 0; i < resultArray.Length; i++)
            {
                results[i] = resultArray[i].ToString() == "1";
            }
            return results;
        }

        // Fallback: if Lua script fails, use parallel execution
        var tasks = new Task<object>[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            tasks[i] = _provider.EvalAsync("return redis.call('SETBIT', KEYS[1], ARGV[1], 1)",
                _redisKey, new object[] { positions[i] }.ToList());
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        var fallbackResults = new bool[tasks.Length];
        for (int i = 0; i < tasks.Length; i++)
        {
            fallbackResults[i] = tasks[i].Result.ToString() == "1";
        }
        return fallbackResults;
    }

    protected override async Task<bool[]> GetBitsAsync(long[] positions)
    {
        // Use Lua script for batch execution
        const string luaScript = @"
local results = {}
for i, pos in ipairs(ARGV) do
    results[i] = redis.call('GETBIT', KEYS[1], pos)
end
return results";

        var result = await _provider.EvalAsync(luaScript, _redisKey, positions.Cast<object>().ToList()).ConfigureAwait(false);

        if (result is object[] resultArray)
        {
            var results = new bool[resultArray.Length];
            for (int i = 0; i < resultArray.Length; i++)
            {
                results[i] = resultArray[i].ToString() == "1";
            }
            return results;
        }

        // Fallback: if Lua script fails, use parallel execution
        var tasks = new Task<object>[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            tasks[i] = _provider.EvalAsync("return redis.call('GETBIT', KEYS[1], ARGV[1])",
                _redisKey, new object[] { positions[i] }.ToList());
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        var fallbackResults = new bool[tasks.Length];
        for (int i = 0; i < tasks.Length; i++)
        {
            fallbackResults[i] = tasks[i].Result.ToString() == "1";
        }
        return fallbackResults;
    }

    protected override void ClearBits()
        => _provider.KeyDel(_redisKey);

    protected override async Task ClearBitsAsync()
        => await _provider.KeyDelAsync(_redisKey);

    // Single element operations

    public override bool Add(ReadOnlySpan<byte> element)
    {
        var positions = ComputeHash(element);
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
    }
}