using System;

namespace BloomFilter.HashAlgorithms;

public class Adler32 : HashFunction
{
    public override HashMethod Method => HashMethod.Adler32;

    private const int seed32 = 89_478_583;

    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];
        int hashes = 0;
        int salt = 0;

        var adler32 = new Internal.Adler32();

        while (hashes < k)
        {
            adler32.Append(data);
            adler32.Append(BitConverter.GetBytes(hashes + salt++ + seed32));

            var adlerValue = adler32.GetCurrentHashAsUInt32();
            adler32.Reset();

            long hash = BinaryHelper.Rejection(adlerValue, m);

            if (hash != -1)
            {
                positions[hashes++] = hash;
            }
        }
        return positions;
    }
}