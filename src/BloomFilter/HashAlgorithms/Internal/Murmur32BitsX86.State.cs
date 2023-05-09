using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace BloomFilter.HashAlgorithms.Internal;

internal partial class Murmur32BitsX86
{
    private struct State
    {
        /// <summary>
        /// First hash multiplication constant.
        /// </summary>
        private const uint C1 = 0xCC9E2D51U;

        /// <summary>
        /// Second hash multiplication constant.
        /// </summary>
        private const uint C2 = 0x1B873593U;

        /// <summary>
        /// The hash value.
        /// </summary>
        private uint _hash;

        internal State(uint seed)
        {
            _hash = seed;
        }

        internal unsafe void ProcessBlock(ReadOnlySpan<byte> source)
        {
            source = source.Slice(0, BlockSize);

            _hash ^= C2 * BinaryHelper.RotateLeft(C1 * BinaryPrimitives.ReadUInt32LittleEndian(source), 15);
            _hash = (5 * BinaryHelper.RotateLeft(_hash, 13)) + 0xE6546B64;
        }

        internal readonly uint Tail(int length, ReadOnlySpan<byte> remaining)
        {
            uint k1 = 0x00000000U;
            var position = 0;

            uint hash = _hash;

            switch (length & 3)
            {
                case 3:
                    k1 ^= (uint)remaining[position + 2] << 16;
                    k1 ^= (uint)remaining[position + 1] << 8;
                    k1 ^= remaining[position];
                    hash ^= C2 * BinaryHelper.RotateLeft(C1 * k1, 15);
                    break;

                case 2:
                    k1 ^= (uint)remaining[position + 1] << 8;
                    k1 ^= remaining[position];
                    hash ^= C2 * BinaryHelper.RotateLeft(C1 * k1, 15);
                    break;

                case 1:
                    k1 ^= remaining[position];
                    hash ^= C2 * BinaryHelper.RotateLeft(C1 * k1, 15);
                    break;
            }

            return Finalization(hash ^ (uint)length);
        }

        /// <summary>
        /// Finalization mix - force all bits of a hash block to avalanche.
        /// </summary>
        /// <param name="k">The value to mix.</param>
        /// <returns>The mixed value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Finalization(in uint k)
        {
            uint k1 = 0x85EBCA6BU * (k ^ (k >> 16));
            uint k2 = 0xC2B2AE35U * (k1 ^ (k1 >> 13));

            return k2 ^ (k2 >> 16);
        }
    }
}