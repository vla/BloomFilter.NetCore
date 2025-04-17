using System;

namespace BloomFilter.HashAlgorithms;

/// <summary>
/// Implements an Random With FNV1a hash algorithm.
/// </summary>
public class RNGWithFNV1a : RNGWithFNV1
{
    public override HashMethod Method => HashMethod.RNGWithFNV1a;

    public override long Hash(ReadOnlySpan<byte> data)
    {
        return Internal.FNV1a.HashToUInt32(data);
    }
}

/// <summary>
/// Implements an Random With modified FNV hash. Provides better distribution than FNV1 but it's only 32 bit long.
/// </summary>
public class RNGModifiedFNV1 : RNGWithFNV1a
{
    public override HashMethod Method => HashMethod.RNGModifiedFNV1;

    public override long Hash(ReadOnlySpan<byte> data)
    {
        return Internal.ModifiedFNV1.HashToUInt32(data);
    }
}

/// <summary>
/// Implements an Random With FNV1 hash algorithm.
/// </summary>
public class RNGWithFNV1 : HashFunction
{
    public override HashMethod Method => HashMethod.RNGWithFNV1;

    protected const long Prime = 16777619;
    protected const long Init = 2166136261;

    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];

        Random r = new((int)Hash(data));

        for (int i = 0; i < k; i++)
        {
#if NET6_0_OR_GREATER
            positions[i] = r.NextInt64(m);
#else
            positions[i] = r.Next((int)m);
#endif
        }
        return positions;
    }

    public virtual long Hash(ReadOnlySpan<byte> data)
    {
        return Internal.FNV1.HashToUInt32(data);
    }
}