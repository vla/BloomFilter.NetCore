using BloomFilter;
using System;
using Xunit;

namespace BloomFilterTest
{
    public class FluentFilterBuilderTest
    {
        [Fact]
        public void Create_ReturnsNewBuilder()
        {
            var builder = FilterBuilder.Create();
            Assert.NotNull(builder);
        }

        [Fact]
        public void BuildInMemory_WithDefaults_CreatesFilter()
        {
            var filter = FilterBuilder.Create()
                .BuildInMemory();

            Assert.NotNull(filter);
            Assert.IsAssignableFrom<IBloomFilter>(filter);
        }

        [Fact]
        public void WithName_SetsFilterName()
        {
            var filter = FilterBuilder.Create()
                .WithName("TestFilter")
                .BuildInMemory();

            Assert.Equal("TestFilter", filter.Name);
        }

        [Fact]
        public void WithName_Null_ThrowsArgumentException()
        {
            var builder = FilterBuilder.Create();

            // ArgumentException.ThrowIfNullOrWhiteSpace throws ArgumentNullException for null
            Assert.ThrowsAny<ArgumentException>(() => builder.WithName(null!));
        }

        [Fact]
        public void WithName_EmptyOrWhitespace_ThrowsArgumentException()
        {
            var builder = FilterBuilder.Create();

            Assert.ThrowsAny<ArgumentException>(() => builder.WithName(""));
            Assert.ThrowsAny<ArgumentException>(() => builder.WithName("   "));
        }

        [Fact]
        public void ExpectingElements_SetsExpectedElements()
        {
            var filter = FilterBuilder.Create()
                .ExpectingElements(5_000_000)
                .BuildInMemory();

            Assert.NotNull(filter);
            // Filter is created successfully with specified expected elements
        }

        [Fact]
        public void ExpectingElements_ZeroOrNegative_ThrowsArgumentOutOfRangeException()
        {
            var builder = FilterBuilder.Create();

            Assert.Throws<ArgumentOutOfRangeException>(() => builder.ExpectingElements(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.ExpectingElements(-1));
        }

        [Fact]
        public void WithErrorRate_SetsErrorRate()
        {
            var filter = FilterBuilder.Create()
                .WithErrorRate(0.001)
                .BuildInMemory();

            Assert.NotNull(filter);
            // Filter is created successfully with specified error rate
        }

        [Fact]
        public void WithErrorRate_InvalidRange_ThrowsArgumentOutOfRangeException()
        {
            var builder = FilterBuilder.Create();

            Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithErrorRate(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithErrorRate(-0.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithErrorRate(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithErrorRate(1.5));
        }

        [Fact]
        public void UsingHashMethod_SetsHashMethod()
        {
            var filter = FilterBuilder.Create()
                .UsingHashMethod(HashMethod.XXHash3)
                .BuildInMemory();

            Assert.NotNull(filter);
            // Filter is created successfully with specified hash method
        }

        [Fact]
        public void UsingCustomHash_SetsCustomHashFunction()
        {
            var customHash = HashFunction.Functions[HashMethod.Murmur3];

            var filter = FilterBuilder.Create()
                .UsingCustomHash(customHash)
                .BuildInMemory();

            Assert.NotNull(filter);
        }

        [Fact]
        public void UsingCustomHash_Null_ThrowsArgumentNullException()
        {
            var builder = FilterBuilder.Create();

            Assert.Throws<ArgumentNullException>(() => builder.UsingCustomHash(null!));
        }

        [Fact]
        public void ChainedCalls_BuildsCorrectFilter()
        {
            var filter = FilterBuilder.Create()
                .WithName("ChainedFilter")
                .ExpectingElements(10_000_000)
                .WithErrorRate(0.001)
                .UsingHashMethod(HashMethod.XXHash3)
                .BuildInMemory();

            Assert.NotNull(filter);
            Assert.Equal("ChainedFilter", filter.Name);
        }

        [Fact]
        public void BuildInMemory_FilterFunctionsCorrectly()
        {
            var filter = FilterBuilder.Create()
                .ExpectingElements(1000)
                .WithErrorRate(0.01)
                .BuildInMemory();

            // Test basic functionality
            filter.Add("test1");
            filter.Add("test2");

            Assert.True(filter.Contains("test1"));
            Assert.True(filter.Contains("test2"));
            Assert.False(filter.Contains("test3"));
        }

        [Fact]
        public void BuildInMemoryWithCapacity_CreatesFilterWithSpecificCapacity()
        {
            var filter = FilterBuilder.Create()
                .WithName("CapacityFilter")
                .BuildInMemoryWithCapacity(capacity: 1000, hashes: 5);

            Assert.NotNull(filter);
            Assert.Equal("CapacityFilter", filter.Name);
        }

        [Fact]
        public void MultipleBuilds_EachBuildIsIndependent()
        {
            var builder = FilterBuilder.Create()
                .ExpectingElements(1000)
                .WithErrorRate(0.01);

            var filter1 = builder.BuildInMemory();
            var filter2 = builder.BuildInMemory();

            filter1.Add("test1");

            Assert.True(filter1.Contains("test1"));
            Assert.False(filter2.Contains("test1")); // filter2 should be independent
        }

        [Fact]
        public void WithSerializer_UsesCustomSerializer()
        {
            var customSerializer = new DefaultFilterMemorySerializer();

            var filter = FilterBuilder.Create()
                .WithSerializer(customSerializer)
                .BuildInMemory();

            Assert.NotNull(filter);
        }

        [Fact]
        public void WithSerializer_Null_ThrowsArgumentNullException()
        {
            var builder = FilterBuilder.Create();

            Assert.Throws<ArgumentNullException>(() => builder.WithSerializer(null!));
        }

        [Fact]
        public void UsingHashMethod_OverridesCustomHash()
        {
            var customHash = HashFunction.Functions[HashMethod.Murmur3];

            var filter = FilterBuilder.Create()
                .UsingCustomHash(customHash)
                .UsingHashMethod(HashMethod.XXHash3) // Should override custom hash
                .BuildInMemory();

            Assert.NotNull(filter);
        }
    }
}
