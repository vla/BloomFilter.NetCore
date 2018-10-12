using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BloomFilter.Redis.Test
{
    public class RedisBitOperateTest
    {

        [Fact]
        public void NormalTest()
        {

            var operate = new RedisBitOperate("localhost");
            operate.Set("RB1", 0, true);
            Assert.True(operate.Get("RB1", 0));
            operate.Dispose();

        }

        [Fact]
        public void Can_Connection_Has_Not_Close()
        {

            var conn = ConnectionMultiplexer.Connect("localhost");
            Assert.True(conn.IsConnected);

            var operate = new RedisBitOperate(conn);
            operate.Set("RB1", 1, true);
            Assert.True(operate.Get("RB1", 1));
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
            operate.Set("RB1", 2, true);
            Assert.True(operate.Get("RB1", 2));
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

    }
}
