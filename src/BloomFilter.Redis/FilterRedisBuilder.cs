using StackExchange.Redis;

namespace BloomFilter.Redis
{
    public class FilterRedisBuilder : FilterBuilder
    {
        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01);
        }

        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01, hashMethod);
        }

        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01, hashFunction);
        }

        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, double errorRate)
        {
            return Build<T>(configuration, redisKey, expectedElements, errorRate, HashFunctions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis<T>(configuration, redisKey, expectedElements, errorRate, HashFunctions[hashMethod]);
        }

        public static Filter<T> Build<T>(string configuration, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis<T>(configuration, redisKey, expectedElements, errorRate, hashFunction);
        }

        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01);
        }

        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, HashMethod hashMethod)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01, hashMethod);
        }

        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, HashFunction hashFunction)
        {
            return Build<T>(configuration, redisKey, expectedElements, 0.01, hashFunction);
        }

        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, double errorRate)
        {
            return Build<T>(configuration, redisKey, expectedElements, errorRate, HashFunctions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterRedis<T>(configuration, redisKey, expectedElements, errorRate, HashFunctions[hashMethod]);
        }

        public static Filter<T> Build<T>(ConfigurationOptions configuration, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterRedis<T>(configuration, redisKey, expectedElements, errorRate, hashFunction);
        }
    }
}