using Heroes.MpqTool;
using System;
using System.Text;

namespace Heroes.ReplayParser.Decoders
{
    internal class BitPackedDecoder
    {
        private readonly MpqBuffer _mpqBuffer;

        public BitPackedDecoder(MpqBuffer mpqBuffer)
        {
            _mpqBuffer = mpqBuffer;
        }

        /// <summary>
        /// Reads up to 32 bits from the buffer and returns a 32-bit unsigned integer.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public uint ReadBits(int numberOfBits)
        {
            if (numberOfBits > 32)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 33");
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return _mpqBuffer.ReadBits(numberOfBits);
        }

        /// <summary>
        /// Reads up to 64 bits from the buffer and returns a 64-bit unsigned integer.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ulong ReadULongBits(int numberOfBits)
        {
            if (numberOfBits > 64)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 65");
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return _mpqBuffer.ReadULongBits(numberOfBits);
        }

        /// <summary>
        /// Reads 8 bytes (unaligned) from the buffer and returns a 64 bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            return ReadULongBits(64);
        }

        /// <summary>
        /// Reads 4 bytes (unaligned) from the buffer and returns a 32 bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            return ReadBits(32);
        }

        /// <summary>
        /// Reads and allocates string from the buffer. Use <see cref="ReadStringAsSpan"/> to read a string without allocation.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public string ReadString(int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return Encoding.UTF8.GetString(_mpqBuffer.ReadBlob(numberOfBits).Span);
        }

        /// <summary>
        /// Reads a single bit from the buffer as a boolean.
        /// </summary>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            return _mpqBuffer.ReadBits(1) == 1;
        }

        /// <summary>
        /// Reads a number of bits from the buffer as a boolean array.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <returns></returns>
        public bool[] ReadBitArray(int numberOfBits)
        {
            bool[] bitArray = new bool[numberOfBits];

            for (int i = 0; i < bitArray.Length; i++)
                bitArray[i] = ReadBoolean();

            return bitArray;
        }

        /// <summary>
        /// Reads the number of bytes from the buffer.
        /// </summary>
        /// <param name="numberOfBytes">The number of bytes to read.</param>
        /// <returns></returns>
        public ReadOnlyMemory<byte> ReadBytes(int numberOfBytes)
        {
            return _mpqBuffer.ReadBytes(numberOfBytes);
        }

        /// <summary>
        /// Reads string bytes from the buffer. Use <see cref="ReadString"/> to read to allocate a string.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ReadOnlySpan<byte> ReadStringAsSpan(int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return _mpqBuffer.ReadBlob(numberOfBits).Span;
        }
    }
}
