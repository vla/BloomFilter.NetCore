using System;
using System.Buffers.Binary;

namespace BloomFilter.HashAlgorithms;

public class XXHash128 : HashFunction
{
    public override uint[] ComputeHash(ReadOnlySpan<byte> data, uint m, uint k)
    {
        uint[] positions = new uint[k];

#if NET7_0_OR_GREATER
        var hash = Internal.XxHash128.HashToHash128(data);
        uint hash1 = (uint)hash.High64;
        uint hash2 = (uint)(hash.High64 >> 32);
#else
        ReadOnlySpan<byte> hash = Internal.XxHash128.Hash(data);
        uint hash1 = BinaryPrimitives.ReadUInt32BigEndian(hash);
        uint hash2 = BinaryPrimitives.ReadUInt32BigEndian(hash.Slice(4));
#endif
        for (int i = 0; i < k; i++)
        {
            positions[i] = (uint)((hash1 + i * hash2) % m);
        }
        return positions;
    }
}