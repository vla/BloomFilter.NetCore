using StackExchange.Redis;
using System;

namespace BloomFilter.Redis;

/// <summary>
/// BloomFilter Redis Builder - Supports both static factory methods and fluent API
/// </summary>
/// <example>
/// Static API:
/// <code>
/// var filter = FilterRedisBuilder.Build(connectionString, "bloom:users", 10_000_000, 0.001);
/// </code>
///
/// Fluent API:
/// <code>
/// var filter = FilterRedisBuilder.Create()
///     .WithRedisConnection(connectionString)
///     .WithRedisKey("bloom:users")
///     .WithName("UserFilter")
///     .ExpectingElements(10_000_000)
///     .WithErrorRate(0.001)
///     .UsingHashMethod(HashMethod.XXHash3)
///     .BuildRedis();
/// </code>
/// </example>
public class FilterRedisBuilder : FilterBuilder
{
    #region Fluent API - Instance Members

    private string? _redisKey;
    private IRedisBitOperate? _redisBitOperate;
    private string? _connectionString;
    private ConfigurationOptions? _configurationOptions;
    private IConnectionMultiplexer? _connectionMultiplexer;

    /// <summary>
    /// Sets the Redis key for the Bloom Filter
    /// </summary>
    /// <param name="redisKey">The Redis key</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when redisKey is null or whitespace</exception>
    public FilterRedisBuilder WithRedisKey(string redisKey)
    {
        if (string.IsNullOrWhiteSpace(redisKey))
            throw new ArgumentException("Redis key cannot be null or whitespace", nameof(redisKey));
        _redisKey = redisKey;
        return this;
    }

    /// <summary>
    /// Sets the Redis connection using a connection string
    /// </summary>
    /// <param name="connectionString">The Redis connection string</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when connectionString is null or whitespace</exception>
    public FilterRedisBuilder WithRedisConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or whitespace", nameof(connectionString));
        _connectionString = connectionString;
        _configurationOptions = null;
        _connectionMultiplexer = null;
        _redisBitOperate = null;
        return this;
    }

    /// <summary>
    /// Sets the Redis connection using ConfigurationOptions
    /// </summary>
    /// <param name="options">The Redis configuration options</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when options is null</exception>
    public FilterRedisBuilder WithRedisConnection(ConfigurationOptions options)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(options);
#else
        if (options == null)
            throw new ArgumentNullException(nameof(options));
#endif
        _configurationOptions = options;
        _connectionString = null;
        _connectionMultiplexer = null;
        _redisBitOperate = null;
        return this;
    }

    /// <summary>
    /// Sets the Redis connection using an existing IConnectionMultiplexer
    /// </summary>
    /// <param name="connection">The Redis connection multiplexer</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when connection is null</exception>
    public FilterRedisBuilder WithRedisConnection(IConnectionMultiplexer connection)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(connection);
#else
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));
#endif
        _connectionMultiplexer = connection;
        _connectionString = null;
        _configurationOptions = null;
        _redisBitOperate = null;
        return this;
    }

    /// <summary>
    /// Sets a custom IRedisBitOperate implementation
    /// </summary>
    /// <param name="redisBitOperate">The Redis bit operate implementation</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when redisBitOperate is null</exception>
    public FilterRedisBuilder WithRedisBitOperate(IRedisBitOperate redisBitOperate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(redisBitOperate);
#else
        if (redisBitOperate == null)
            throw new ArgumentNullException(nameof(redisBitOperate));
#endif
        _redisBitOperate = redisBitOperate;
        _connectionString = null;
        _configurationOptions = null;
        _connectionMultiplexer = null;
        return this;
    }

    /// <summary>
    /// Builds a Redis-backed Bloom Filter with the configured settings
    /// </summary>
    /// <returns>A new IBloomFilter instance backed by Redis</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing</exception>
    public IBloomFilter BuildRedis()
    {
        if (string.IsNullOrWhiteSpace(_redisKey))
            throw new InvalidOperationException("Redis key must be specified using WithRedisKey()");

        // Get hash function from base class configuration
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];

        // Determine Redis bit operate instance
        IRedisBitOperate bitOperate;
        if (_redisBitOperate != null)
        {
            bitOperate = _redisBitOperate;
        }
        else if (_connectionMultiplexer != null)
        {
            bitOperate = new RedisBitOperate(_connectionMultiplexer);
        }
        else if (_configurationOptions != null)
        {
            bitOperate = new RedisBitOperate(_configurationOptions);
        }
        else if (!string.IsNullOrWhiteSpace(_connectionString))
        {
            bitOperate = new RedisBitOperate(_connectionString!);
        }
        else
        {
            throw new InvalidOperationException("Redis connection must be specified using WithRedisConnection() or WithRedisBitOperate()");
        }

        return new FilterRedis(_name, bitOperate, _redisKey!, _expectedElements, _errorRate, hashFunction);
    }

    /// <summary>
    /// Builds a Redis-backed Bloom Filter with explicit capacity and hash count (advanced usage)
    /// </summary>
    /// <param name="capacity">The bit array capacity</param>
    /// <param name="hashes">The number of hash functions</param>
    /// <returns>A new IBloomFilter instance backed by Redis</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing</exception>
    public IBloomFilter BuildRedisWithCapacity(long capacity, int hashes)
    {
        if (string.IsNullOrWhiteSpace(_redisKey))
            throw new InvalidOperationException("Redis key must be specified using WithRedisKey()");

        // Get hash function from base class configuration
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];

        // Determine Redis bit operate instance
        IRedisBitOperate bitOperate;
        if (_redisBitOperate != null)
        {
            bitOperate = _redisBitOperate;
        }
        else if (_connectionMultiplexer != null)
        {
            bitOperate = new RedisBitOperate(_connectionMultiplexer);
        }
        else if (_configurationOptions != null)
        {
            bitOperate = new RedisBitOperate(_configurationOptions);
        }
        else if (!string.IsNullOrWhiteSpace(_connectionString))
        {
            bitOperate = new RedisBitOperate(_connectionString!);
        }
        else
        {
            throw new InvalidOperationException("Redis connection must be specified using WithRedisConnection() or WithRedisBitOperate()");
        }

        return new FilterRedis(_name, bitOperate, _redisKey!, capacity, hashes, hashFunction);
    }

    #endregion

    #region Static Factory Methods (Legacy API - Backward Compatible)
    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(string configuration, string redisKey, long expectedElements, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(configuration, redisKey, expectedElements, 0.01, name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="hashMethod">The hash method.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(string configuration, string redisKey, long expectedElements, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(configuration, redisKey, expectedElements, 0.01, hashMethod, name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(string configuration, string redisKey, long expectedElements, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(configuration, redisKey, expectedElements, 0.01, hashFunction, name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(string configuration, string redisKey, long expectedElements, double errorRate, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(configuration, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3], name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashMethod">The hash method.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(string configuration, string redisKey, long expectedElements, double errorRate, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return new FilterRedis(name, new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(string configuration, string redisKey, long expectedElements, double errorRate, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return new FilterRedis(name, new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, hashFunction);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, long expectedElements, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(configuration, redisKey, expectedElements, 0.01, name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="hashMethod">The hash method.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, long expectedElements, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(configuration, redisKey, expectedElements, 0.01, hashMethod, name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, long expectedElements, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(configuration, redisKey, expectedElements, 0.01, hashFunction, name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, long expectedElements, double errorRate, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(configuration, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3], name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashMethod">The hash method.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, long expectedElements, double errorRate, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return new FilterRedis(name, new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, long expectedElements, double errorRate, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return new FilterRedis(name, new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, hashFunction);
    }

    /// <summary>
    /// Builds the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="hashMethod">The hash method.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, long expectedElements, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(connection, redisKey, expectedElements, 0.01, hashMethod, name);
    }

    /// <summary>
    /// Builds the specified connectionn.
    /// </summary>
    /// <param name="connectionn">The connectionn.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IConnectionMultiplexer connectionn, string redisKey, long expectedElements, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(connectionn, redisKey, expectedElements, 0.01, hashFunction, name);
    }

    /// <summary>
    /// Builds the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, long expectedElements, double errorRate, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(connection, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3], name);
    }

    /// <summary>
    /// Builds the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, long expectedElements, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(connection, redisKey, expectedElements, 0.01, HashFunction.Functions[HashMethod.Murmur3], name);
    }

    /// <summary>
    /// Builds the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashMethod">The hash method.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, long expectedElements, double errorRate, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return new FilterRedis(name, new RedisBitOperate(connection), redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
    }

    /// <summary>
    /// Builds the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, long expectedElements, double errorRate, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return new FilterRedis(name, new RedisBitOperate(connection), redisKey, expectedElements, errorRate, hashFunction);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="redisBitOperate">The redis bit operate.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="hashMethod">The hash method.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, long expectedElements, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(redisBitOperate, redisKey, expectedElements, 0.01, hashMethod, name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="redisBitOperate">The redis bit operate.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, long expectedElements, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(redisBitOperate, redisKey, expectedElements, 0.01, hashFunction, name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="redisBitOperate">The redis bit operate.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, long expectedElements, double errorRate, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(redisBitOperate, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3], name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="redisBitOperate">The redis bit operate.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, long expectedElements, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return Build(redisBitOperate, redisKey, expectedElements, 0.01, HashFunction.Functions[HashMethod.Murmur3], name);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="redisBitOperate">The redis bit operate.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashMethod">The hash method.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, long expectedElements, double errorRate, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return new FilterRedis(name, redisBitOperate, redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
    }

    /// <summary>
    /// Creates a BloomFilter Redis for the specified expected element
    /// </summary>
    /// <param name="redisBitOperate">The redis bit operate.</param>
    /// <param name="redisKey">The redis key.</param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, long expectedElements, double errorRate, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultRedisName)
    {
        return new FilterRedis(name, redisBitOperate, redisKey, expectedElements, errorRate, hashFunction);
    }

    #endregion
}