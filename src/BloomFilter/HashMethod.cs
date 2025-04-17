namespace BloomFilter;

/// <summary>
/// Hash Methods
/// </summary>
public enum HashMethod : int
{
    LCGWithFNV1 = 0,
    LCGWithFNV1a = 1,
    LCGModifiedFNV1 = 2,

    RNGWithFNV1 = 100,
    RNGWithFNV1a = 101,
    RNGModifiedFNV1 = 102,

    CRC32 = 200,
    CRC64 = 201,
    Adler32 = 202,

    Murmur3 = 300,
    Murmur32BitsX86 = 301,
    Murmur128BitsX64 = 302,
    Murmur128BitsX86 = 303,

    SHA1 = 400,
    SHA256 = 401,
    SHA384 = 402,
    SHA512 = 403,

    XXHash32 = 500,
    XXHash64 = 501,
    XXHash3 = 502,
    XXHash128 = 503,
}