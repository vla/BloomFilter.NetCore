using System;

namespace BloomFilter.HashAlgorithms;

public partial class Crc64 : HashFunction
{
    public override HashMethod Method => HashMethod.CRC64;

    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];

        var hash = Internal.Crc64.HashToUInt64(data);

        uint hash1 = (uint)hash;
        uint hash2 = (uint)(hash >> 32);

        for (int i = 0; i < k; i++)
        {
            positions[i] = ((hash1 + (i * hash2)) % m);
        }
        return positions;
    }
}