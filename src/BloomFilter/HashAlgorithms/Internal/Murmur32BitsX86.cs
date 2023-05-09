using System;
using System.Buffers.Binary;

namespace BloomFilter.HashAlgorithms.Internal;

/// <summary>
///  Provides an implementation of the MurmurHash 32 bits x86 algorithm.
///  https://en.wikipedia.org/wiki/MurmurHash
/// </summary>
internal partial class Murmur32BitsX86 : NonCryptoHashAlgorithm
{
    private const int HashSize = sizeof(uint);
    private const int BlockSize = sizeof(uint);

    private readonly uint _seed;
    private State _state;
    private byte[]? _holdback;
    private int _length;

    public Murmur32BitsX86() : base(HashSize)
    {
    }

    public Murmur32BitsX86(uint seed)
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
        int held = _length & 0x03;

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
        uint hash = GetCurrentHashAsUInt32();
        BinaryPrimitives.WriteUInt32LittleEndian(destination, hash);
    }

    /// <summary>Gets the current computed hash value without modifying accumulated state.</summary>
    /// <returns>The hash value for the data already provided.</returns>
    public uint GetCurrentHashAsUInt32()
    {
        int remainingLength = _length & 0x03;
        ReadOnlySpan<byte> remaining = ReadOnlySpan<byte>.Empty;

        if (remainingLength > 0)
        {
            remaining = new ReadOnlySpan<byte>(_holdback, 0, remainingLength);
        }

        return _state.Tail(_length, remaining);
    }

    public static uint HashToUInt32(ReadOnlySpan<byte> source, uint seed = 0)
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
}