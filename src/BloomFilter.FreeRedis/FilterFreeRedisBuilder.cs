using FreeRedis;
using System;

namespace BloomFilter.FreeRedis;

/// <summary>
/// BloomFilter FreeRedis Builder - Supports both static factory methods and fluent API
/// </summary>
/// <example>
/// Fluent API:
/// <code>
/// var filter = FilterFreeRedisBuilder.Create()
///     .WithRedisClient(redisClient)
///     .WithRedisKey("bloom:users")
///     .WithName("UserFilter")
///     .ExpectingElements(10_000_000)
///     .WithErrorRate(0.001)
///     .UsingHashMethod(HashMethod.XXHash3)
///     .BuildFreeRedis();
/// </code>
/// </example>
public class FilterFreeRedisBuilder : FilterBuilder
{
    #region Fluent API - Instance Members

    private string? _redisKey;
    private RedisClient? _client;

    /// <summary>
    /// Sets the Redis key for the Bloom Filter
    /// </summary>
    /// <param name="redisKey">The Redis key</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when redisKey is null or whitespace</exception>
    public FilterFreeRedisBuilder WithRedisKey(string redisKey)
    {
        if (string.IsNullOrWhiteSpace(redisKey))
            throw new ArgumentException("Redis key cannot be null or whitespace", nameof(redisKey));
        _redisKey = redisKey;
        return this;
    }

    /// <summary>
    /// Sets the FreeRedis client
    /// </summary>
    /// <param name="client">The RedisClient instance</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null</exception>
    public FilterFreeRedisBuilder WithRedisClient(RedisClient client)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(client);
#else
        if (client == null)
            throw new ArgumentNullException(nameof(client));
#endif
        _client = client;
        return this;
    }

    /// <summary>
    /// Builds a FreeRedis-backed Bloom Filter with the configured settings
    /// </summary>
    /// <returns>A new IBloomFilter instance backed by FreeRedis</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing</exception>
    public IBloomFilter BuildFreeRedis()
    {
        if (string.IsNullOrWhiteSpace(_redisKey))
            throw new InvalidOperationException("Redis key must be specified using WithRedisKey()");

        if (_client == null)
            throw new InvalidOperationException("FreeRedis client must be specified using WithRedisClient()");

        // Get hash function from base class configuration
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];

        return new FilterFreeRedis(_name, _client, _redisKey!, _expectedElements, _errorRate, hashFunction);
    }

    /// <summary>
    /// Builds a FreeRedis-backed Bloom Filter with explicit capacity and hash count (advanced usage)
    /// </summary>
    /// <param name="capacity">The bit array capacity</param>
    /// <param name="hashes">The number of hash functions</param>
    /// <returns>A new IBloomFilter instance backed by FreeRedis</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing</exception>
    public IBloomFilter BuildFreeRedisWithCapacity(long capacity, int hashes)
    {
        if (string.IsNullOrWhiteSpace(_redisKey))
            throw new InvalidOperationException("Redis key must be specified using WithRedisKey()");

        if (_client == null)
            throw new InvalidOperationException("FreeRedis client must be specified using WithRedisClient()");

        // Get hash function from base class configuration
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];

        return new FilterFreeRedis(_name, _client, _redisKey!, capacity, hashes, hashFunction);
    }

    #endregion
}
