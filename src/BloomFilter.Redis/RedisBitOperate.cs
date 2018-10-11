using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BloomFilter.Redis
{
    public class RedisBitOperate : IRedisBitOperate, IDisposable
    {
        private volatile ConnectionMultiplexer _connection;
        private readonly ConfigurationOptions _configurationOptions;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        private bool allowRelease;

        public RedisBitOperate(string configuration)
            : this(ConfigurationOptions.Parse(configuration))
        {
        }

        public RedisBitOperate(ConfigurationOptions configurationOptions)
        {
            _configurationOptions = configurationOptions;
        }


        public RedisBitOperate(ConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public void Clear(string redisKey)
        {
            Database().KeyDelete(redisKey);
        }

        public bool Get(string redisKey, int position)
        {
            return Database().StringGetBit(redisKey, position);
        }

        public bool[] Get(string redisKey, int[] positions)
        {
            var results = new Task<bool>[positions.Length];

            var batch = Database().CreateBatch();

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = batch.StringGetBitAsync(redisKey, positions[i]);
            }

            batch.Execute();
            batch.WaitAll(results);

            return results.Select(r => r.Result).ToArray();
        }

        public bool[] Set(string redisKey, int[] positions, bool value)
        {
            var results = new Task<bool>[positions.Length];

            var batch = Database().CreateBatch();

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = batch.StringSetBitAsync(redisKey, positions[i], true);
            }

            batch.Execute();
            batch.WaitAll(results);

            return results.Select(r => r.Result).ToArray();
        }

        public bool Set(string redisKey, int position, bool value)
        {
            return Database().StringSetBit(redisKey, position, value);
        }

        public void Dispose()
        {
            if (allowRelease)
            {
                _connection?.Dispose();
            }
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
                if (_connection == null)
                {
                    _connection = ConnectionMultiplexer.Connect(_configurationOptions);
                    allowRelease = true;
                }
                else
                {
                    if (allowRelease)
                    {
                        _connection.Dispose();
                    }

                    _connection = ConnectionMultiplexer.Connect(_connection.Configuration);
                    allowRelease = true;
                }
            }
            finally
            {
                _connectionLock.Release();
            }

            return _connection;
        }
    }
}