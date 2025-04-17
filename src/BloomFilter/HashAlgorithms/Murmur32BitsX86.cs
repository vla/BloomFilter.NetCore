using System;

namespace BloomFilter.HashAlgorithms;

/// <summary>
/// Building a Better Bloom Filter" by Adam Kirsch and Michael Mitzenmacher,
/// https://www.eecs.harvard.edu/~michaelm/postscripts/tr-02-05.pdf
/// </summary>
public class Murmur32BitsX86 : HashFunction
{
    public override HashMethod Method => HashMethod.Murmur32BitsX86;

    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];
        var hash1 = Internal.Murmur32BitsX86.HashToUInt32(data);
        var hash2 = Internal.Murmur32BitsX86.HashToUInt32(data, hash1);
        for (int i = 0; i < k; i++)
        {
            positions[i] = (hash1 + i * hash2) % m;
        }
        return positions;
    }
}