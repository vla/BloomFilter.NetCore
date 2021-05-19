using StackExchange.Redis;

namespace BloomFilter.Redis
{
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
        /// <returns></returns>
        public static IBloomFilter Build(string configuration, string redisKey, int expectedElements)
        {
            return Build(configuration, redisKey, expectedElements, 0.01);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(string configuration, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build(configuration, redisKey, expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(string configuration, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build(configuration, redisKey, expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static IBloomFilter Build(string configuration, string redisKey, int expectedElements, double errorRate)
        {
            return Build(configuration, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(string configuration, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis(new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(string configuration, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis(new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <returns></returns>
        public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, int expectedElements)
        {
            return Build(configuration, redisKey, expectedElements, 0.01);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build(configuration, redisKey, expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build(configuration, redisKey, expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, int expectedElements, double errorRate)
        {
            return Build(configuration, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis(new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(ConfigurationOptions configuration, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis(new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, hashFunction);
        }


        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build(connection, redisKey, expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Builds the specified connectionn.
        /// </summary>
        /// <param name="connectionn">The connectionn.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IConnectionMultiplexer connectionn, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build(connectionn, redisKey, expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, int expectedElements, double errorRate)
        {
            return Build(connection, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, int expectedElements)
        {
            return Build(connection, redisKey, expectedElements, 0.01, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis(new RedisBitOperate(connection), redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IConnectionMultiplexer connection, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis(new RedisBitOperate(connection), redisKey, expectedElements, errorRate, hashFunction);
        }


        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build(redisBitOperate, redisKey, expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build(redisBitOperate, redisKey, expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, double errorRate)
        {
            return Build(redisBitOperate, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements)
        {
            return Build(redisBitOperate, redisKey, expectedElements, 0.01, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis(redisBitOperate, redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis(redisBitOperate, redisKey, expectedElements, errorRate, hashFunction);
        }

















        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, double errorRate)
        {
            return Build<T>(configuration, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis<T>(new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis<T>(new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, double errorRate)
        {
            return Build<T>(configuration, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis<T>(new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis<T>(new RedisBitOperate(configuration), redisKey, expectedElements, errorRate, hashFunction);
        }


        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IConnectionMultiplexer connection, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build<T>(connection, redisKey, expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Builds the specified connectionn.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionn">The connectionn.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IConnectionMultiplexer connectionn, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build<T>(connectionn, redisKey, expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IConnectionMultiplexer connection, string redisKey, int expectedElements, double errorRate)
        {
            return Build<T>(connection, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IConnectionMultiplexer connection, string redisKey, int expectedElements)
        {
            return Build<T>(connection, redisKey, expectedElements, 0.01, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IConnectionMultiplexer connection, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis<T>(new RedisBitOperate(connection), redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Builds the specified connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IConnectionMultiplexer connection, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis<T>(new RedisBitOperate(connection), redisKey, expectedElements, errorRate, hashFunction);
        }


        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build<T>(redisBitOperate, redisKey, expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build<T>(redisBitOperate, redisKey, expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, double errorRate)
        {
            return Build<T>(redisBitOperate, redisKey, expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements)
        {
            return Build<T>(redisBitOperate, redisKey, expectedElements, 0.01, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis<T>(redisBitOperate, redisKey, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter Redis for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="redisKey">The redis key.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(IRedisBitOperate redisBitOperate, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis<T>(redisBitOperate, redisKey, expectedElements, errorRate, hashFunction);
        }
    }
}