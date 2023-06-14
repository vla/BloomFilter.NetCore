namespace BloomFilter;

/// <summary>
/// Hash Methods
/// </summary>
public enum HashMethod
{
    LCGWithFNV1,
    LCGWithFNV1a,
    LCGModifiedFNV1,
    RNGWithFNV1,
    RNGWithFNV1a,
    RNGModifiedFNV1,
    CRC32,
    CRC64,
    Adler32,
    Murmur3,
    Murmur32BitsX86,
    Murmur128BitsX64,
    Murmur128BitsX86,
    SHA1,
    SHA256,
    SHA384,
    SHA512,
    XXHash32,
    XXHash64,
    XXHash3,
    XXHash128,
}