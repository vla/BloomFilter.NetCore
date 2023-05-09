using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace BloomFilter.HashAlgorithms.Internal;

internal partial class Murmur128BitsX86
{
    private struct State
    {
        private const uint C1 = 0x239b961b;
        private const uint C2 = 0xab0e9789;
        private const uint C3 = 0x38b34ae5;
        private const uint C4 = 0xa1e38b93;

        private uint _hash1;
        private uint _hash2;
        private uint _hash3;
        private uint _hash4;

        internal State(uint seed)
        {
            _hash1 = _hash2 = _hash3 = _hash4 = seed;
        }

        internal unsafe void ProcessBlock(ReadOnlySpan<byte> source)
        {
            source = source.Slice(0, BlockSize);

            uint k1 = BinaryPrimitives.ReadUInt32LittleEndian(source);
            uint k2 = BinaryPrimitives.ReadUInt32LittleEndian(source.Slice(sizeof(uint)));
            uint k3 = BinaryPrimitives.ReadUInt32LittleEndian(source.Slice(sizeof(uint) * 2));
            uint k4 = BinaryPrimitives.ReadUInt32LittleEndian(source.Slice(sizeof(uint) * 3));

            k1 *= C1; k1 = BinaryHelper.RotateLeft(k1, 15); k1 *= C2; _hash1 ^= k1;

            _hash1 = BinaryHelper.RotateLeft(_hash1, 19); _hash1 += _hash2; _hash1 = _hash1 * 5 + 0x561ccd1b;

            k2 *= C2; k2 = BinaryHelper.RotateLeft(k2, 16); k2 *= C3; _hash2 ^= k2;

            _hash2 = BinaryHelper.RotateLeft(_hash2, 17); _hash2 += _hash3; _hash2 = _hash2 * 5 + 0x0bcaa747;

            k3 *= C3; k3 = BinaryHelper.RotateLeft(k3, 17); k3 *= C4; _hash3 ^= k3;

            _hash3 = BinaryHelper.RotateLeft(_hash3, 15); _hash3 += _hash4; _hash3 = _hash3 * 5 + 0x96cd1c35;

            k4 *= C4; k4 = BinaryHelper.RotateLeft(k4, 18); k4 *= C1; _hash4 ^= k4;

            _hash4 = BinaryHelper.RotateLeft(_hash4, 13); _hash4 += _hash1; _hash4 = _hash4 * 5 + 0x32ac3b17;
        }

        internal readonly uint[] Tail(int length, ReadOnlySpan<byte> remaining)
        {
            uint h1 = _hash1;
            uint h2 = _hash2;
            uint h3 = _hash3;
            uint h4 = _hash4;

            uint k1 = 0;
            uint k2 = 0;
            uint k3 = 0;
            uint k4 = 0;

            // determine how many bytes we have left to work with based on length
            switch (length & 15)
            {
                case 15: k4 ^= (uint)remaining[14] << 16; goto case 14;
                case 14: k4 ^= (uint)remaining[13] << 8; goto case 13;
                case 13: k4 ^= (uint)remaining[12] << 0; goto case 12;
                case 12: k3 ^= (uint)remaining[11] << 24; goto case 11;
                case 11: k3 ^= (uint)remaining[10] << 16; goto case 10;
                case 10: k3 ^= (uint)remaining[9] << 8; goto case 9;
                case 9: k3 ^= (uint)remaining[8] << 0; goto case 8;
                case 8: k2 ^= (uint)remaining[7] << 24; goto case 7;
                case 7: k2 ^= (uint)remaining[6] << 16; goto case 6;
                case 6: k2 ^= (uint)remaining[5] << 8; goto case 5;
                case 5: k2 ^= (uint)remaining[4] << 0; goto case 4;
                case 4: k1 ^= (uint)remaining[3] << 24; goto case 3;
                case 3: k1 ^= (uint)remaining[2] << 16; goto case 2;
                case 2: k1 ^= (uint)remaining[1] << 8; goto case 1;
                case 1: k1 ^= (uint)remaining[0] << 0; break;
            }

            h4 ^= BinaryHelper.RotateLeft(k4 * C4, 18) * C1;
            h3 ^= BinaryHelper.RotateLeft(k3 * C3, 17) * C4;
            h2 ^= BinaryHelper.RotateLeft(k2 * C2, 16) * C3;
            h1 ^= BinaryHelper.RotateLeft(k1 * C1, 15) * C2;

            return Finalization(length, h1, h2, h3, h4);
        }

        private static uint[] Finalization(int length, uint h1, uint h2, uint h3, uint h4)
        {
            uint len = (uint)length;
            // pipelining friendly algorithm
            h1 ^= len; h2 ^= len; h3 ^= len; h4 ^= len;

            h1 += (h2 + h3 + h4);
            h2 += h1; h3 += h1; h4 += h1;

            h1 = FMix(h1);
            h2 = FMix(h2);
            h3 = FMix(h3);
            h4 = FMix(h4);

            h1 += (h2 + h3 + h4);
            h2 += h1; h3 += h1; h4 += h1;

            return new[] { h1, h2, h3, h4 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint FMix(uint h)
        {
            h = (h ^ (h >> 16)) * 0x85ebca6b;
            h = (h ^ (h >> 13)) * 0xc2b2ae35;
            return h ^ (h >> 16);
        }
    }
}