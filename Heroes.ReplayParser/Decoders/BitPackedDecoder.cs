using Heroes.MpqTool;
using System;
using System.Text;

namespace Heroes.ReplayParser.Decoders
{
    internal class BitPackedDecoder : BitPackedBuffer
    {
        public BitPackedDecoder(MpqBuffer mpqBuffer)
            : base(mpqBuffer)
        {
        }

        /// <summary>
        /// Reads 8 unaligned bytes from the buffer and returns a 64 bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            return ReadULongBits(64);
        }

        /// <summary>
        /// Reads 4 unaligned bytes from the buffer and returns a 32 bit unsigned integer.
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

            return Encoding.UTF8.GetString(ReadBlob(numberOfBits).Span);
        }

        /// <summary>
        /// Reads string bytes from the buffer. Use <see cref="ReadString"/> to read to allocate a string.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ReadOnlySpan<char> ReadStringAsSpan(int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            ReadOnlySpan<byte> blobSpan = ReadBlob(numberOfBits).Span;
            Span<char> charValues = new char[blobSpan.Length];

            Encoding.UTF8.GetChars(blobSpan, charValues);
            return charValues;
        }

        /// <summary>
        /// Reads a single bit from the buffer as a boolean.
        /// </summary>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            return ReadBits(1) == 1;
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
    }
}
