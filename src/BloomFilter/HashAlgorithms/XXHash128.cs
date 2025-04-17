using System;
using System.Buffers.Binary;

namespace BloomFilter.HashAlgorithms;

public class XXHash128 : HashFunction
{
    public override HashMethod Method => HashMethod.XXHash128;

    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];

#if NET7_0_OR_GREATER
        var hash = Internal.XxHash128.HashToHash128(data);
        var hash1 = (uint)hash.High64;
        var hash2 = (uint)(hash.High64 >> 32);
#else
        ReadOnlySpan<byte> hash = Internal.XxHash128.Hash(data);
        var hash1 = BinaryPrimitives.ReadUInt32BigEndian(hash);
        var hash2 = BinaryPrimitives.ReadUInt32BigEndian(hash.Slice(4));
#endif
        for (int i = 0; i < k; i++)
        {
            positions[i] = ((hash1 + i * hash2) % m);
        }
        return positions;
    }
}