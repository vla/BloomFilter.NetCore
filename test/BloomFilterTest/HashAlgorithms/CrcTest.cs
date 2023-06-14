using System;
using System.Collections.Generic;
using BloomFilter.HashAlgorithms.Internal;
using FluentAssertions;
using Xunit;

namespace BloomFilterTest.HashAlgorithms
{
    public class CrcTest
    {
        private static readonly byte[] buffer = new byte[] { 0x49, 0x74, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x6D, 0x69, 0x73, 0x74, 0x61, 0x6B, 0x65, 0x20, 0x74, 0x6F, 0x20, 0x74, 0x68, 0x69, 0x6E, 0x6B, 0x20, 0x79, 0x6F, 0x75, 0x20, 0x63, 0x61, 0x6E, 0x20, 0x73, 0x6F, 0x6C, 0x76, 0x65, 0x20, 0x61, 0x6E, 0x79, 0x20, 0x6D, 0x61, 0x6A, 0x6F, 0x72, 0x20, 0x70, 0x72, 0x6F, 0x62, 0x6C, 0x65, 0x6D, 0x73, 0x20, 0x6A, 0x75, 0x73, 0x74, 0x20, 0x77, 0x69, 0x74, 0x68, 0x20, 0x70, 0x6F, 0x74, 0x61, 0x74, 0x6F, 0x65, 0x73 };

        [Theory]
        [MemberData(nameof(Crc64TestData))]
        public void Crc64_Append<ExpectedChecksum>(TestPayloadParameter<ExpectedChecksum> parameter)
        {
            var range = parameter.TestRange;

            Crc64 crc = new();

            Span<byte> data = parameter.TestPayload;

            if (!range.Equals(Range.All))
            {
                data = parameter.TestPayload[range];
            }

            while (data.Length > 0)
            {
                var n = Random.Shared.Next(1, data.Length);
                var d = data.Slice(0, n);
                crc.Append(d);
                data = data.Slice(n);
            }

            ulong actual = crc.GetCurrentHashAsUInt64();

            var expected = (ulong)(object)parameter.ExpectedValue;

            $"0x{actual:X4}".Should().Be($"0x{expected:X4}");
        }


        [Theory]
        [MemberData(nameof(Crc32TestData))]
        public void Crc32_Append<ExpectedChecksum>(TestPayloadParameter<ExpectedChecksum> parameter)
        {
            var range = parameter.TestRange;

            Crc32 crc = new();

            Span<byte> data = parameter.TestPayload;

            if (!range.Equals(Range.All))
            {
                data = parameter.TestPayload[range];
            }

            while (data.Length > 0)
            {
                var n = Random.Shared.Next(1, data.Length);
                var d = data.Slice(0, n);
                crc.Append(d);
                data = data.Slice(n);
            }

            uint actual = crc.GetCurrentHashAsUInt32();

            var expected = (uint)(object)parameter.ExpectedValue;

            $"0x{actual:X4}".Should().Be($"0x{expected:X4}");
        }


        public static IEnumerable<object[]> Crc64TestData()
        {
            yield return new object[] { new TestPayloadParameter<ulong>(buffer, .., 4970656839403593224) { TestScenario = "CRC 64, Checksum of all bytes" } };
            yield return new object[] { new TestPayloadParameter<ulong>(buffer, 3..14, 7160332793041206217) { TestScenario = "CRC 64, Checksum of bytes 3-14" } };
            yield return new object[] { new TestPayloadParameter<ulong>(buffer, 9..14, 11684299407097624602) { TestScenario = "CRC 64, Checksum of bytes 9-14" } };
            yield return new object[] { new TestPayloadParameter<ulong>(buffer, 70..76, 7838111982916138213) { TestScenario = "CRC 64, Checksum of bytes 70-76" } };
        }

        public static IEnumerable<object[]> Crc32TestData()
        {
            yield return new object[] { new TestPayloadParameter<uint>(buffer, .., 0xCBE25E99) { TestScenario = "CRC 32, Checksum of all bytes" } };
            yield return new object[] { new TestPayloadParameter<uint>(buffer, 3..14, 0xA0D118A0) { TestScenario = "CRC 32, Checksum of bytes 3-14" } };
            yield return new object[] { new TestPayloadParameter<uint>(buffer, 9..14, 0xDEF4CFE9) { TestScenario = "CRC 32, Checksum of bytes 9-14" } };
            yield return new object[] { new TestPayloadParameter<uint>(buffer, 70..76, 0xF373B43B) { TestScenario = "CRC 32, Checksum of bytes 70-76" } };
        }
    }
}