using System;
using System.Buffers.Binary;

namespace BloomFilter.HashAlgorithms.Internal;

/// <summary>
///  Provides an implementation of the MurmurHash 128 bits x64 algorithm.
///  https://en.wikipedia.org/wiki/MurmurHash
/// </summary>
internal partial class Murmur128BitsX64 : NonCryptoHashAlgorithm
{
    private const int HashSize = sizeof(ulong) * 2;
    private const int BlockSize = sizeof(ulong) * 2;

    private readonly uint _seed;
    private State _state;
    private byte[]? _holdback;
    private int _length;

    public Murmur128BitsX64() : base(HashSize)
    {
    }

    public Murmur128BitsX64(uint seed)
       : base(HashSize)
    {
        _seed = seed;
        Reset();
    }

    public override void Reset()
    {
        _state = new State(_seed);
        _length = 0;
    }

    public override void Append(ReadOnlySpan<byte> source)
    {
        int held = _length & 0x0F;

        if (held != 0)
        {
            int remain = BlockSize - held;

            if (source.Length >= remain)
            {
                source.Slice(0, remain).CopyTo(_holdback.AsSpan(held));
                _state.ProcessBlock(_holdback);

                source = source.Slice(remain);
                _length += remain;
            }
            else
            {
                source.CopyTo(_holdback.AsSpan(held));
                _length += source.Length;
                return;
            }
        }

        while (source.Length >= BlockSize)
        {
            _state.ProcessBlock(source);
            source = source.Slice(BlockSize);
            _length += BlockSize;
        }

        if (source.Length > 0)
        {
            _holdback ??= new byte[BlockSize];
            source.CopyTo(_holdback);
            _length += source.Length;
        }
    }

    protected override void GetCurrentHashCore(Span<byte> destination)
    {
        ulong[] hash = GetCurrentHashAsUInt128Array();
        Fill(destination, ref hash);
    }

#if NET7_0_OR_GREATER
    /// <summary>Gets the current computed hash value without modifying accumulated state.</summary>
    /// <returns>The hash value for the data already provided.</returns>
    public UInt128 GetCurrentHashAsUInt128()
    {
        ulong[] current = GetCurrentHashAsUInt128Array();
        return new UInt128(current[1], current[2]);
    }
#endif

    /// <summary>Gets the current computed hash value without modifying accumulated state.</summary>
    /// <returns>The hash value for the data already provided.</returns>
    public ulong[] GetCurrentHashAsUInt128Array()
    {
        int remainingLength = _length & 0x0F;
        ReadOnlySpan<byte> remaining = ReadOnlySpan<byte>.Empty;

        if (remainingLength > 0)
        {
            remaining = new ReadOnlySpan<byte>(_holdback, 0, remainingLength);
        }

        return _state.Tail(_length, remaining);
    }

    public static ulong[] HashToUInt128Array(ReadOnlySpan<byte> source, uint seed = 0)
    {
        int totalLength = source.Length;

        State state = new(seed);

        while (source.Length >= BlockSize)
        {
            state.ProcessBlock(source);
            source = source.Slice(BlockSize);
        }

        return state.Tail(totalLength, source);
    }

    private static void Fill(Span<byte> destination, ref ulong[] hash)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(destination, hash[0]);
        BinaryPrimitives.WriteUInt64LittleEndian(destination.Slice(sizeof(ulong)), hash[1]);
    }
}