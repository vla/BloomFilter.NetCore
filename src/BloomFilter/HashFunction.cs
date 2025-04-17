using BloomFilter.HashAlgorithms;
using System;
using System.Collections.Generic;

namespace BloomFilter;

/// <summary>
/// An implemented to provide custom hash functions.
/// </summary>
public abstract class HashFunction
{
    /// <summary>
    /// The hash functions
    /// </summary>
    public static readonly IReadOnlyDictionary<HashMethod, HashFunction> Functions = new Dictionary<HashMethod, HashFunction>
    {
        { HashMethod.LCGWithFNV1,new LCGWithFNV() },
        { HashMethod.LCGWithFNV1a,new LCGWithFNV1a() },
        { HashMethod.LCGModifiedFNV1,new LCGModifiedFNV1() },

        { HashMethod.RNGWithFNV1,new RNGWithFNV1() },
        { HashMethod.RNGWithFNV1a,new RNGWithFNV1a() },
        { HashMethod.RNGModifiedFNV1,new RNGModifiedFNV1() },

        { HashMethod.CRC32,new Crc32() },
        { HashMethod.CRC64,new Crc64() },
        { HashMethod.Adler32,new Adler32() },

        { HashMethod.Murmur3,new Murmur32BitsX86() },
        { HashMethod.Murmur32BitsX86,new Murmur32BitsX86() },
        { HashMethod.Murmur128BitsX64,new Murmur128BitsX64() },
        { HashMethod.Murmur128BitsX86,new Murmur128BitsX86() },

        { HashMethod.SHA1,new HashCryptoSHA1() },
        { HashMethod.SHA256,new HashCryptoSHA256() },
        { HashMethod.SHA384,new HashCryptoSHA384() },
        { HashMethod.SHA512,new HashCryptoSHA512() },

        { HashMethod.XXHash32,new XXHash32() },
        { HashMethod.XXHash64,new XXHash64() },
        { HashMethod.XXHash3,new XXHash3() },
        { HashMethod.XXHash128,new XXHash128() },
    };

    /// <summary>
    /// Hash Method
    /// </summary>
    public abstract HashMethod Method { get; }

    /// <summary>
    /// Hashes the specified value.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="m">integer output range.</param>
    /// <param name="k">number of hashes to be computed.</param>
    /// <returns>
    /// int array of hashes hash values
    /// </returns>
    public abstract long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k);
}