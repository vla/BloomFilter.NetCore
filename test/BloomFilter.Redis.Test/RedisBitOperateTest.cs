using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BloomFilter.Redis.Test
{
    public class RedisBitOperateTest
    {

        [Fact]
        public void NormalTest()
        {

            var operate = new RedisBitOperate("localhost");
            operate.Set("RB-NormalTest", 0, true);
            Assert.True(operate.Get("RB-NormalTest", 0));
            operate.Dispose();

        }

        [Fact]
        public async Task NormalTestAsync()
        {

            var operate = new RedisBitOperate("localhost");
            await operate.SetAsync("RB-NormalTestAsync", 0, true);
            Assert.True(await operate.GetAsync("RB-NormalTestAsync", 0));
            operate.Dispose();

        }

        [Fact]
        public void Can_Connection_Has_Not_Close()
        {

            var conn = ConnectionMultiplexer.Connect("localhost");
            Assert.True(conn.IsConnected);

            var operate = new RedisBitOperate(conn);
            operate.Set("Can_Connection_Has_Not_Close", 1, true);
            Assert.True(operate.Get("Can_Connection_Has_Not_Close", 1));
            operate.Dispose();

            Assert.True(conn.IsConnected);

        }

        [Fact]
        public async Task Can_Connection_Has_Not_CloseAsync()
        {

            var conn = ConnectionMultiplexer.Connect("localhost");
            Assert.True(conn.IsConnected);

            var operate = new RedisBitOperate(conn);
            await operate.SetAsync("Can_Connection_Has_Not_CloseAsync", 1, true);
            Assert.True(await operate.GetAsync("Can_Connection_Has_Not_CloseAsync", 1));
            operate.Dispose();

            Assert.True(conn.IsConnected);

        }

        [Fact]
        public void Can_Close_With_New_Connection()
        {

            var conn = ConnectionMultiplexer.Connect("localhost");
            Assert.True(conn.IsConnected);
            conn.Dispose();
            Assert.False(conn.IsConnected);

            var operate = new RedisBitOperate(conn);
            operate.Set("Can_Close_With_New_Connection", 2, true);
            Assert.True(operate.Get("Can_Close_With_New_Connection", 2));
            operate.Dispose();

        }

        [Fact]
        public async Task Can_Close_With_New_ConnectionAsync()
        {

            var conn = ConnectionMultiplexer.Connect("localhost");
            Assert.True(conn.IsConnected);
            conn.Dispose();
            Assert.False(conn.IsConnected);

            var operate = new RedisBitOperate(conn);
            await operate.SetAsync("Can_Close_With_New_ConnectionAsync", 2, true);
            Assert.True(await operate.GetAsync("Can_Close_With_New_ConnectionAsync", 2));
            operate.Dispose();

        }

        [Fact]
        public void Can_The_Batch_Result()
        {
            var operate = new RedisBitOperate(ConfigurationOptions.Parse("localhost"));
            string key = "RB:Batch";
            operate.Clear(key);

            var positions = new int[] { 5, 6, 7, 8, 9, 10, 11, 12 };

            var results = operate.Set(key, positions, true);
            Assert.False(results.All(r => r));

            results = operate.Set(key, positions, true);
            Assert.True(results.All(r => r));

            results = operate.Get(key, positions);
            Assert.True(results.All(r => r));

            operate.Clear(key);
            results = operate.Get(key, positions);
            Assert.False(results.All(r => r));

            operate.Dispose();
        }

        [Fact]
        public async Task Can_The_Batch_ResultAsync()
        {
            var operate = new RedisBitOperate(ConfigurationOptions.Parse("localhost"));
            string key = "RB:BatchAsync";
            await operate.ClearAsync(key);

            var positions = new int[] { 5, 6, 7, 8, 9, 10, 11, 12 };

            var results = await operate.SetAsync(key, positions, true);
            Assert.False(results.All(r => r));

            results = await operate.SetAsync(key, positions, true);
            Assert.True(results.All(r => r));

            results = await operate.GetAsync(key, positions);
            Assert.True(results.All(r => r));

            await operate.ClearAsync(key);
            results = await operate.GetAsync(key, positions);
            Assert.False(results.All(r => r));

            operate.Dispose();
        }

    }
}
