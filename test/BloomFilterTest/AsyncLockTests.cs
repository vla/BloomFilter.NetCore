using BloomFilter;
using Xunit;

namespace BloomFilterTest
{
    [Collection("AsyncLock Tests")]
    [CollectionDefinition("AsyncLock Tests", DisableParallelization = false)]
    public class AsyncLockTests
    {
        [Fact]
        public void TestMaxCount()
        {
            using var mutex = new AsyncLock(2);
            Assert.Equal(0, mutex.GetRemainingCount());
            Assert.Equal(mutex.MaxCount, mutex.GetCurrentCount());
            using (var myLock = mutex.Acquire())
            {
                Assert.Equal(1, mutex.GetRemainingCount());
                Assert.Equal(1, mutex.GetCurrentCount());
                using var myLock2 = mutex.Acquire();
                Assert.Equal(mutex.MaxCount, mutex.GetRemainingCount());
                Assert.Equal(0, mutex.GetCurrentCount());
            }
            Assert.Equal(0, mutex.GetRemainingCount());
            Assert.Equal(mutex.MaxCount, mutex.GetCurrentCount());
        }

        [Fact]
        public void TestLock()
        {
            var mutex = new AsyncLock();
            Assert.Equal(0, mutex.GetRemainingCount());
            Assert.Equal(1, mutex.GetCurrentCount());
            using (var myLock = mutex.Acquire())
            {
                Assert.Equal(1, mutex.GetRemainingCount());
                Assert.Equal(0, mutex.GetCurrentCount());
            }
            Assert.Equal(0, mutex.GetRemainingCount());
            Assert.Equal(1, mutex.GetCurrentCount());
        }
    }
}
