using System;

namespace BloomFilter.HashAlgorithms;

/// <summary>
/// Implements an Linear Congruential Generator With  FNV1a hash algorithm.
/// </summary>
public class LCGWithFNV1a : LCGWithFNV
{
    public override long Hash(ReadOnlySpan<byte> data)
    {
        return Internal.FNV1a.HashToUInt32(data);
    }
}

/// <summary>
/// Implements an Linear Congruential Generator With  modified FNV hash. Provides better distribution than FNV1 but it's only 32 bit long.
/// </summary>
public class LCGModifiedFNV1 : LCGWithFNV1a
{
    public override long Hash(ReadOnlySpan<byte> data)
    {
        return Internal.ModifiedFNV1.HashToUInt32(data);
    }
}

/// <summary>
/// Implements an Linear Congruential Generator With FNV1 hash algorithm.
/// </summary>
public class LCGWithFNV : HashFunction
{
    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long multiplier = 0x5DEECE66DL;
        long addend = 0xBL;
        long mask = (1L << 48) - 1;

        // Generate int from byte Array using the FNV hash
        long reduced = Math.Abs(Hash(data));
        // Make number positive
        // Handle the special case: smallest negative number is itself as the
        // absolute value
        if (reduced == int.MaxValue)
            reduced = 42;

        // Calculate hashes numbers iteratively
        long[] positions = new long[k];
        long seed = reduced;
        for (int i = 0; i < k; i++)
        {
            //http://en.wikipedia.org/wiki/Linear_congruential_generator
            // LCG formula: x_i+1 = (multiplier * x_i + addend) mod mask
            seed = (seed * multiplier + addend) & mask;
            positions[i] = BinaryHelper.RightMove(seed, (48 - 30)) % m;
        }
        return positions;
    }

    public virtual long Hash(ReadOnlySpan<byte> data)
    {
        return Internal.FNV1.HashToUInt32(data);
    }
}