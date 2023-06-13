using System;

namespace BloomFilter.HashAlgorithms;

public partial class Crc32 : HashFunction
{
    private const int seed32 = 89478583;

    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];
        int hashes = 0;
        int salt = 0;

        var crc32 = new Internal.Crc32();

        while (hashes < k)
        {
            crc32.Append(data);
            crc32.Append(BitConverter.GetBytes(hashes + salt++ + seed32));

            var crc = crc32.GetCurrentHashAsUInt32();
            crc32.Reset();

            long hash = BinaryHelper.Rejection(crc, m);
            if (hash != -1)
            {
                positions[hashes++] = hash;
            }
        }
        return positions;
    }
}