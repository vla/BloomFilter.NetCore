using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace BloomFilter.HashAlgorithms.Internal;

internal partial class Murmur128BitsX64
{
    private struct State
    {
        private const ulong C1 = 0x87c37b91114253d5UL;
        private const ulong C2 = 0x4cf5ad432745937fUL;

        private ulong _hash1;
        private ulong _hash2;

        internal State(uint seed)
        {
            _hash1 = _hash2 = seed;
        }

        internal unsafe void ProcessBlock(ReadOnlySpan<byte> source)
        {
            source = source.Slice(0, BlockSize);

            ulong k1 = BinaryPrimitives.ReadUInt64LittleEndian(source);
            ulong k2 = BinaryPrimitives.ReadUInt64LittleEndian(source.Slice(sizeof(ulong)));

            // a variant of original algorithm optimized for processor instruction pipelining

            k1 *= C1; k1 = BinaryHelper.RotateLeft(k1, 31); k1 *= C2; _hash1 ^= k1;

            _hash1 = BinaryHelper.RotateLeft(_hash1, 27); _hash1 += _hash2; _hash1 = _hash1 * 5 + 0x52dce729;

            k2 *= C2; k2 = BinaryHelper.RotateLeft(k2, 33); k2 *= C1; _hash2 ^= k2;

            _hash2 = BinaryHelper.RotateLeft(_hash2, 31); _hash2 += _hash1; _hash2 = _hash2 * 5 + 0x38495ab5;
        }

        internal readonly ulong[] Tail(int length, ReadOnlySpan<byte> remaining)
        {
            ulong hash1 = _hash1;
            ulong hash2 = _hash2;

            ulong k1 = 0, k2 = 0;

            // determine how many bytes we have left to work with based on length
            switch (length & 15)
            {
                case 15: k2 ^= (ulong)remaining[14] << 48; goto case 14;
                case 14: k2 ^= (ulong)remaining[13] << 40; goto case 13;
                case 13: k2 ^= (ulong)remaining[12] << 32; goto case 12;
                case 12: k2 ^= (ulong)remaining[11] << 24; goto case 11;
                case 11: k2 ^= (ulong)remaining[10] << 16; goto case 10;
                case 10: k2 ^= (ulong)remaining[9] << 8; goto case 9;
                case 9: k2 ^= (ulong)remaining[8] << 0; goto case 8;
                case 8: k1 ^= (ulong)remaining[7] << 56; goto case 7;
                case 7: k1 ^= (ulong)remaining[6] << 48; goto case 6;
                case 6: k1 ^= (ulong)remaining[5] << 40; goto case 5;
                case 5: k1 ^= (ulong)remaining[4] << 32; goto case 4;
                case 4: k1 ^= (ulong)remaining[3] << 24; goto case 3;
                case 3: k1 ^= (ulong)remaining[2] << 16; goto case 2;
                case 2: k1 ^= (ulong)remaining[1] << 8; goto case 1;
                case 1: k1 ^= (ulong)remaining[0] << 0; break;
            }

            hash2 ^= BinaryHelper.RotateLeft(k2 * C2, 33) * C1;
            hash1 ^= BinaryHelper.RotateLeft(k1 * C1, 31) * C2;

            return Finalization(length, hash1, hash2);
        }

        private static ulong[] Finalization(int length, ulong hash1, ulong hash2)
        {
            var H1 = hash1;
            var H2 = hash2;

            ulong len = (ulong)length;
            H1 ^= len;
            H2 ^= len;

            H1 += H2;
            H2 += H1;

            H1 = FMix(H1);
            H2 = FMix(H2);

            H1 += H2;
            H2 += H1;

            return new[] { H1, H2 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong FMix(ulong h)
        {
            h = (h ^ (h >> 33)) * 0xff51afd7ed558ccd;
            h = (h ^ (h >> 33)) * 0xc4ceb9fe1a85ec53;
            return (h ^ (h >> 33));
        }
    }
}