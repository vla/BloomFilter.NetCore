using System;

namespace BloomFilter.HashAlgorithms;

/// <summary>
/// Implements an Random With FNV1a hash algorithm.
/// </summary>
public class RNGWithFNV1a : RNGWithFNV1
{
    public override uint Hash(ReadOnlySpan<byte> data)
    {
        return Internal.FNV1a.HashToUInt32(data);
    }
}

/// <summary>
/// Implements an Random With modified FNV hash. Provides better distribution than FNV1 but it's only 32 bit long.
/// </summary>
public class RNGModifiedFNV1 : RNGWithFNV1a
{
    public override uint Hash(ReadOnlySpan<byte> data)
    {
        return Internal.ModifiedFNV1.HashToUInt32(data);
    }
}

/// <summary>
/// Implements an Random With FNV1 hash algorithm.
/// </summary>
public class RNGWithFNV1 : HashFunction
{
    protected const uint Prime = 16777619;
    protected const uint Init = 2166136261;

    public override uint[] ComputeHash(ReadOnlySpan<byte> data, uint m, uint k)
    {
        uint[] positions = new uint[k];

        Random r = new((int)Hash(data));

        for (int i = 0; i < k; i++)
        {
#if NET6_0_OR_GREATER
            positions[i] = (uint)r.NextInt64(m);
#else
            positions[i] = (uint)r.Next((int)m);
#endif
        }
        return positions;
    }

    public virtual uint Hash(ReadOnlySpan<byte> data)
    {
        return Internal.FNV1.HashToUInt32(data);
    }
}