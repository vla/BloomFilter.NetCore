using StackExchange.Redis;

namespace BloomFilter.Redis;

/// <summary>
/// BloomFilter Redis Builder
/// </summary>
/// <seealso cref="BloomFilter.FilterBuilder" />
public class FilterRedisBuilder : FilterBuilder
{
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
}