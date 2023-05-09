using System;

namespace BloomFilter.HashAlgorithms;

/// <summary>
/// Building a Better Bloom Filter" by Adam Kirsch and Michael Mitzenmacher,
/// https://www.eecs.harvard.edu/~michaelm/postscripts/tr-02-05.pdf
/// </summary>
public class Murmur32BitsX86 : HashFunction
{
    public override uint[] ComputeHash(ReadOnlySpan<byte> data, uint m, uint k)
    {
        uint[] positions = new uint[k];
        uint hash1 = Internal.Murmur32BitsX86.HashToUInt32(data);
        uint hash2 = Internal.Murmur32BitsX86.HashToUInt32(data, hash1);
        for (int i = 0; i < k; i++)
        {
            positions[i] = (uint)((hash1 + i * hash2) % m);
        }
        return positions;
    }
}