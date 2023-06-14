using System;

namespace BloomFilterTest.HashAlgorithms
{
    public class TestPayloadParameter<TExpected>
    {
        public byte[] TestPayload { get; set; }
        public Range TestRange { get; set; }
        public TExpected ExpectedValue { get; set; }
        public string TestScenario { get; set; }

        public TestPayloadParameter(byte[] payload, Range range, TExpected expected)
        {
            TestPayload = payload;
            TestRange = range;
            ExpectedValue = expected;
            TestScenario = "";
        }

        public TestPayloadParameter(byte[] payload, TExpected expected)
        {
            TestPayload = payload;
            TestRange = Range.All;
            ExpectedValue = expected;
            TestScenario = "";
        }
    }
}