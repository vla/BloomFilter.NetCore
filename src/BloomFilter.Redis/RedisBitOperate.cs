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
        private volatile IConnectionMultiplexer? _connection;
        private readonly ConfigurationOptions? _configurationOptions;
        private readonly SemaphoreSlim _connectionLock = new(initialCount: 1, maxCount: 1);

        //allow the release of connection
        private bool allowRelease;
        private const int TimeoutMilliseconds = 5 * 1000;

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
            _configurationOptions = configurationOptions ?? throw new ArgumentNullException(nameof(configurationOptions));
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

        public async Task ClearAsync(string redisKey)
        {
            var db = await DatabaseAsync();
            await db.KeyDeleteAsync(redisKey).ConfigureAwait(false);
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

            if (!Task.WaitAll(results, TimeoutMilliseconds)) throw new TimeoutException();

            return results.Select(r => r.Result).ToArray();
        }

        public async Task<bool> GetAsync(string redisKey, int position)
        {
            var db = await DatabaseAsync();
            return await db.StringGetBitAsync(redisKey, position).ConfigureAwait(false);
        }

        public async Task<bool[]> GetAsync(string redisKey, int[] positions)
        {
            var results = new bool[positions.Length];

            var db = await DatabaseAsync();


            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = await db.StringGetBitAsync(redisKey, positions[i]).ConfigureAwait(false);
            }


            return results;
        }

        public bool[] Set(string redisKey, int[] positions, bool value)
        {
            var results = new Task<bool>[positions.Length];

            var batch = Database().CreateBatch();

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = batch.StringSetBitAsync(redisKey, positions[i], value);
            }

            batch.Execute();
            if (!Task.WaitAll(results, TimeoutMilliseconds)) throw new TimeoutException();

            return results.Select(r => r.Result).ToArray();
        }

        public async Task<bool[]> SetAsync(string redisKey, int[] positions, bool value)
        {
            var results = new bool[positions.Length];

            var db = await DatabaseAsync();

            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = await db.StringSetBitAsync(redisKey, positions[i], value).ConfigureAwait(false);
            }

            return results;
        }

        public bool Set(string redisKey, int position, bool value)
        {
            return Database().StringSetBit(redisKey, position, value);
        }

        public async Task<bool> SetAsync(string redisKey, int position, bool value)
        {
            var db = await DatabaseAsync();
            return await db.StringSetBitAsync(redisKey, position, value).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (allowRelease)
            {
                _connection?.Dispose();
            }
        }

        private IDatabase Database(int? db = null)
        {
            return GetConnection().GetDatabase(db ?? -1);
        }

        private async Task<IDatabase> DatabaseAsync(int? db = null)
        {
            var conn = await GetConnectionAsync();
            return conn.GetDatabase(db ?? -1);
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
                    _connection = ConnectionMultiplexer.Connect(_configurationOptions!);
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

        private async Task<IConnectionMultiplexer> GetConnectionAsync()
        {

            if (_connection != null && _connection.IsConnected)
                return _connection;

            await _connectionLock.WaitAsync();

            try
            {
                if (_connection != null && _connection.IsConnected)
                    return _connection;

                if (_connection == null)
                {
                    _connection = await ConnectionMultiplexer.ConnectAsync(_configurationOptions!);
                    allowRelease = true;
                }
                else
                {
                    if (allowRelease)
                    {
                        _connection.Dispose();
                    }

                    _connection = await ConnectionMultiplexer.ConnectAsync(_connection.Configuration);
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