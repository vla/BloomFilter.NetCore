using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BloomFilter.Redis
{
    /// <summary>
    /// Redis Bit Operate
    /// </summary>
    /// <seealso cref="IRedisBitOperate" />
    public class RedisBitOperate : IRedisBitOperate
    {
        private volatile IConnectionMultiplexer _connection;
        private readonly ConfigurationOptions _configurationOptions;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        //allow the release of connection
        private bool allowRelease;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBitOperate"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public RedisBitOperate(string configuration)
            : this(ConfigurationOptions.Parse(configuration))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBitOperate"/> class.
        /// </summary>
        /// <param name="configurationOptions">The <see cref="ConfigurationOptions"/> options.</param>
        public RedisBitOperate(ConfigurationOptions configurationOptions)
        {
            _configurationOptions = configurationOptions;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBitOperate"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="IConnectionMultiplexer"/>.</param>
        public RedisBitOperate(IConnectionMultiplexer connection)
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

        private IConnectionMultiplexer GetConnection()
        {
            if (_connection != null && _connection.IsConnected)
                return _connection;

            try
            {
                _connectionLock.Wait();

                if (_connection != null && _connection.IsConnected)
                    return _connection;

                //Create a new connection instance
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

                    //If the constructor parameter USES ConnectionMultiplexer,
                    //the configuration of ConnectionMultiplexer is reused
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