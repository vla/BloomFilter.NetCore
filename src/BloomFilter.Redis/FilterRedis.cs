using StackExchange.Redis;
using System;
using System.Threading;

namespace BloomFilter.Redis
{
    public class FilterRedis<T> : Filter<T>, IDisposable
    {
        private readonly string _redisKey;
        private volatile ConnectionMultiplexer _connection;
        private readonly ConfigurationOptions _configurationOptions;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        public FilterRedis(ConfigurationOptions configurationOptions, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
            : base(expectedElements, errorRate, hashFunction)
        {
            _configurationOptions = configurationOptions;
            _redisKey = redisKey;
        }

        public FilterRedis(ConfigurationOptions configurationOptions, string redisKey, int capacity, int hashes, HashFunction hashFunction)
            : base(capacity, hashes, hashFunction)
        {
            _configurationOptions = configurationOptions;
            _redisKey = redisKey;
        }


        public FilterRedis(string configuration, string redisKey, int expectedElements, double errorRate, HashFunction hashFunction)
           : this(ConfigurationOptions.Parse(configuration), redisKey, expectedElements, errorRate, hashFunction)
        {
        }

        public FilterRedis(string configuration, string redisKey, int capacity, int hashes, HashFunction hashFunction)
            : this(ConfigurationOptions.Parse(configuration), redisKey, capacity, hashes, hashFunction)
        {
        }

        public override bool Add(byte[] element)
        {
            bool added = false;
            var positions = ComputeHash(element);

            var db = Database();

            foreach (int position in positions)
            {
                if (!db.StringGetBit(_redisKey, position))
                {
                    added = true;
                    db.StringSetBit(_redisKey, position, true);
                }
            }
            return added;
        }

        public override void Clear()
        {
            Database().KeyDelete(_redisKey);
        }

        public override bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);
            var db = Database();

            foreach (int position in positions)
            {
                if (!db.StringGetBit(_redisKey, position))
                    return false;
            }
            return true;
        }

        private IDatabase Database(int? db = default(int?))
        {
            return GetConnection().GetDatabase(db ?? -1);
        }

        private ConnectionMultiplexer GetConnection()
        {
            if (_connection != null && _connection.IsConnected) return _connection;

            _connectionLock.Wait();

            if (_connection != null && _connection.IsConnected) return _connection;

            try
            {
                if (_connection != null)
                {
                    _connection.Dispose();
                }

                _connection = ConnectionMultiplexer.Connect(_configurationOptions);
            }
            finally
            {
                _connectionLock.Release();
            }

            return _connection;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
