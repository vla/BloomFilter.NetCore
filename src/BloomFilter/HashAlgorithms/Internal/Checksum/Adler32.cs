using System;
using System.Security.Cryptography;

namespace BloomFilter.HashAlgorithms.Checksum
{
    /// <summary>
    /// Computes Adler32 checksum for a stream of data
    /// </summary>
    internal class Adler32 : HashAlgorithm, IChecksum
    {
        private uint _current;
        private static readonly uint BASE = 65521;

        /// <summary>
        /// Initializes a new instance of the <see cref="Adler32"/> class.
        /// </summary>
        public Adler32()
        {
            HashSizeValue = 64;
            Reset();
        }

        /// <summary>
        /// Returns the data checksum computed so far.
        /// </summary>
        public long Value => _current;

        public override void Initialize()
        {
            Reset();
        }

        /// <summary>
        /// Resets the data checksum as if no update was ever called.
        /// </summary>
        public void Reset()
        {
            _current = 1;
        }

        /// <summary>
        /// Adds one byte to the data checksum.
        /// </summary>
        /// <param name="b">the data value to add. The high byte of the int is ignored.</param>
        public void Update(int b)
        {
            uint s1 = _current & 0xFFFF;
            uint s2 = _current >> 16;

            s1 = (s1 + ((uint)b & 0xFF)) % BASE;
            s2 = (s1 + s2) % BASE;

            _current = (s2 << 16) + s1;
        }

        /// <summary>
        /// Updates the data checksum with the bytes taken from the array.
        /// </summary>
        /// <param name="buffer">buffer an array of bytes</param>
        /// <exception cref="ArgumentNullException">buffer</exception>
        public void Update(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            Update(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Adds the byte array to the data checksum.
        /// </summary>
        /// <param name="buffer">The buffer which contains the data</param>
        /// <param name="offset">The offset in the buffer where the data starts</param>
        /// <param name="count">the number of data bytes to add.</param>
        /// <exception cref="ArgumentNullException">buffer</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// offset - cannot be less than zero
        /// or
        /// offset - not a valid index into buffer
        /// or
        /// count - cannot be less than zero
        /// or
        /// count - exceeds buffer size
        /// </exception>
        public void Update(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "cannot be less than zero");
            }

            if (offset >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "not a valid index into buffer");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "cannot be less than zero");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "exceeds buffer size");
            }

            uint s1 = _current & 0xFFFF;
            uint s2 = _current >> 16;

            while (count > 0)
            {
                int n = 3800;
                if (n > count)
                {
                    n = count;
                }
                count -= n;
                while (--n >= 0)
                {
                    s1 += (uint)(buffer[offset++] & 0xff);
                    s2 += s1;
                }
                s1 %= BASE;
                s2 %= BASE;
            }

            _current = (s2 << 16) | s1;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (ibStart < 0 || ibStart > array.Length) throw new ArgumentOutOfRangeException(nameof(ibStart));
            if (ibStart + cbSize > array.Length) throw new ArgumentOutOfRangeException(nameof(cbSize));

            Update(array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(Value);
        }

    }
}