using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            var positions = ComputeHash(element);
            var db = Database();

            var results = new Task<bool>[positions.Length];

            var batch = db.CreateBatch();

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = batch.StringSetBitAsync(_redisKey, positions[i], true);
            }

            batch.Execute();
            batch.WaitAll(results);

            return results.Any(a => !a.Result);
        }

        public override void Clear()
        {
            Database().KeyDelete(_redisKey);
        }

        public override bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);
            var db = Database();

            var results = new Task<bool>[positions.Length];

            var batch = db.CreateBatch();

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = batch.StringGetBitAsync(_redisKey, positions[i]);
            }

            batch.Execute();
            batch.WaitAll(results);

            return results.All(a => a.Result);
        }

        private IDatabase Database(int? db = default(int?))
        {
            return GetConnection().GetDatabase(db ?? -1);
        }

        private ConnectionMultiplexer GetConnection()
        {
            if (_connection != null && _connection.IsConnected)
                return _connection;

            _connectionLock.Wait();

            if (_connection != null && _connection.IsConnected)
                return _connection;

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
