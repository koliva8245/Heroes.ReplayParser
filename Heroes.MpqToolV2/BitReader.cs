using System;
using System.Buffers.Binary;
using System.Text;

namespace Heroes.MpqToolV2
{
    /// <summary>
    /// Contains the extension methods for Span and ReadOnlySpan.
    /// </summary>
    public static class BitReader
    {
        private static int _bitIndex;
        private static byte _currentByte;

        /// <summary>
        /// Gets or sets the current byte index.
        /// </summary>
        public static int Index { get; set; } = 0;

        /// <summary>
        /// Gets or sets the <see cref="EndianType"/>.
        /// </summary>
        public static EndianType EndianType { get; set; } = EndianType.LittleEndian;

        /// <summary>
        /// Resets the index, bitIndex, and currentByte to 0.
        /// </summary>
        public static void ResetIndex()
        {
            Index = 0;
            _bitIndex = 0;
            _currentByte = 0;
        }

        /// <summary>
        /// Reads up to 32 bits from the read-only span as an uint.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static uint ReadBits(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits > 32)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 33");
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return EndianType == EndianType.LittleEndian ? GetValueFromBits(source, numberOfBits) : BinaryPrimitives.ReverseEndianness(GetValueFromBits(source, numberOfBits));
        }

        /// <summary>
        /// Reads up to 64 bits from the read-only span as an ulong.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static ulong ReadULongBits(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits > 64)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 65");
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return EndianType == EndianType.LittleEndian ? GetULongValueFromBits(source, numberOfBits) : BinaryPrimitives.ReverseEndianness(GetULongValueFromBits(source, numberOfBits));
        }

        /// <summary>
        /// Reads up to 64 bits from the read-only span as an long.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static long ReadLongBits(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits > 64)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 65");
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return EndianType == EndianType.LittleEndian ? GetLongValueFromBits(source, numberOfBits) : BinaryPrimitives.ReverseEndianness(GetLongValueFromBits(source, numberOfBits));
        }

        /// <summary>
        /// Read a number of bits from the read-only span as an array of booleans.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <returns></returns>
        public static bool[] ReadBitArray(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            bool[] bitArray = new bool[numberOfBits];

            for (int i = 0; i < bitArray.Length; i++)
                bitArray[i] = source.ReadBoolean();

            return bitArray;
        }

        /// <summary>
        /// Read a single bit from the read-only span as a boolean.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static bool ReadBoolean(this ReadOnlySpan<byte> source)
        {
            int bytePosition = _bitIndex & 7;

            if (bytePosition == 0)
            {
                _currentByte = source.ReadByte();
            }

            bool bit = ((_currentByte >> bytePosition) & 1) == 1;

            _bitIndex++;

            return bit;
        }

        /// <summary>
        /// Reads 2 aligned bytes from the read-only span as an ushort.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static ushort ReadUInt16Aligned(this ReadOnlySpan<byte> source)
        {
            ushort value = BinaryPrimitives.ReadUInt16LittleEndian(source.Slice(Index, 2));
            Index += 2;

            return value;
        }

        /// <summary>
        /// Reads 2 aligned bytes from the read-only span as a short.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static short ReadInt16Aligned(this ReadOnlySpan<byte> source)
        {
            short value = BinaryPrimitives.ReadInt16LittleEndian(source.Slice(Index, 2));
            Index += 2;

            return value;
        }

        /// <summary>
        /// Reads 4 aligned bytes from the read-only span as an uint.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static uint ReadUInt32Aligned(this ReadOnlySpan<byte> source)
        {
            uint value = BinaryPrimitives.ReadUInt32LittleEndian(source.Slice(Index, 4));
            Index += 4;

            return value;
        }

        /// <summary>
        /// Reads 4 aligned bytes from the read-only span as a int.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static int ReadInt32Aligned(this ReadOnlySpan<byte> source)
        {
            int value = BinaryPrimitives.ReadInt32LittleEndian(source.Slice(Index, 4));
            Index += 4;

            return value;
        }

        /// <summary>
        /// Reads 4 unaligned bytes from the read-only span as an uint.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static uint ReadUInt32Unaligned(this ReadOnlySpan<byte> source)
        {
            return source.ReadBits(32);
        }

        /// <summary>
        /// Reads 4 unaligned bytes from the read-only span as an int.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static int ReadInt32Unaligned(this ReadOnlySpan<byte> source)
        {
            return (int)source.ReadBits(32);
        }

        /// <summary>
        /// Reads 8 aligned bytes from the read-only span as a ulong.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static ulong ReadUInt64Aligned(this ReadOnlySpan<byte> source)
        {
            ulong value = BinaryPrimitives.ReadUInt64LittleEndian(source.Slice(Index, 8));
            Index += 8;

            return value;
        }

        /// <summary>
        /// Reads 8 aligned bytes from the read-only span as a long.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static long ReadInt64Aligned(this ReadOnlySpan<byte> source)
        {
            long value = BinaryPrimitives.ReadInt64LittleEndian(source.Slice(Index, 8));
            Index += 8;

            return value;
        }

        /// <summary>
        /// Reads 8 unaligned bytes from the read-only span as an ulong.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static ulong ReadUInt64Unaligned(this ReadOnlySpan<byte> source)
        {
            return source.ReadULongBits(64);
        }

        /// <summary>
        /// Reads 8 unaligned bytes from the read-only span as a long.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static long ReadInt64Unaligned(this ReadOnlySpan<byte> source)
        {
            return source.ReadLongBits(64);
        }

        /// <summary>
        /// Reads a signed integer of variable length.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static long ReadVInt(this ReadOnlySpan<byte> source)
        {
            byte dataByte = source.ReadByte();
            int negative = dataByte & 1;
            long result = (dataByte >> 1) & 0x3f;
            int bits = 6;

            while ((dataByte & 0x80) != 0)
            {
                dataByte = source.ReadByte();
                result |= ((long)dataByte & 0x7f) << bits;
                bits += 7;
            }

            return negative < 0 ? -negative : result;
        }

        /// <summary>
        /// Returns the number of bytes read for a vInt.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static ReadOnlySpan<byte> ReadBytesForVInt(this ReadOnlySpan<byte> source)
        {
            int count = 1;

            byte dataByte = source.ReadByte();
            long result = (dataByte >> 1) & 0x3f;
            int bits = 6;

            while ((dataByte & 0x80) != 0)
            {
                count++;
                dataByte = source.ReadByte();
                result |= ((long)dataByte & 0x7f) << bits;
                bits += 7;
            }

            Index -= count;

            return source.ReadBytes(count);
        }

        /// <summary>
        /// Reads one byte.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static byte ReadByte(this ReadOnlySpan<byte> source)
        {
            byte value = source[Index];
            Index++;

            return value;
        }

        /// <summary>
        /// Reads a number of bytes.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns></returns>
        public static ReadOnlySpan<byte> ReadBytes(this ReadOnlySpan<byte> source, int count)
        {
            ByteAlign();

            ReadOnlySpan<byte> value = source.Slice(Index, count);
            Index += count;

            return value;
        }

        /// <summary>
        /// Reads a number of bits from the read-only span as a UTF-8 string.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static string ReadBlobAsString(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return Encoding.UTF8.GetString(ReadBlob(source, numberOfBits));
        }

        /// <summary>
        /// Reads a number of bits from the read-only span as a UTF-8 string.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static string ReadStringFromBits(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return Encoding.UTF8.GetString(BitConverter.GetBytes(source.ReadBits(numberOfBits)));
        }

        /// <summary>
        /// Reads a number of bytes from the read-only span as a UTF-8 string.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBytes">The number of bytes to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static string ReadStringFromBytes(this ReadOnlySpan<byte> source, int numberOfBytes)
        {
            if (numberOfBytes < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBytes), "Number of bytes must be greater than 0");

            ReadOnlySpan<byte> bytes = source.ReadBytes(numberOfBytes);

            if (EndianType == EndianType.BigEndian)
            {
                return Encoding.UTF8.GetString(bytes);
            }
            else
            {
                return string.Create(numberOfBytes, bytes.ToArray(), (buffer, value) =>
                {
                    Encoding.UTF8.GetChars(value, buffer);

                    buffer.Reverse();
                });
            }
        }

        /// <summary>
        /// Reads 4 bytes from the read-only span as a UTF-8 string.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static string ReadStringFromFourBytes(this ReadOnlySpan<byte> source)
        {
            int valueInt = source.ReadInt32Aligned();

            if (EndianType == EndianType.BigEndian)
            {
                return string.Create(4, valueInt, (buffer, value) =>
                {
                    Encoding.UTF8.GetChars(BitConverter.GetBytes(value), buffer);
                });

            }
            else
            {

                return string.Create(4, valueInt, (buffer, value) =>
                {
                    Encoding.UTF8.GetChars(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value)), buffer);
                });
            }
        }

        /// <summary>
        /// Reads a number of bytes.
        /// </summary>
        /// <param name="source">The span of bytes to read.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns></returns>
        public static Span<byte> ReadBytes(this Span<byte> source, int count)
        {
            Span<byte> value = source.Slice(Index, count);
            Index += count;

            return value;
        }

        private static void ByteAlign()
        {
            if ((_bitIndex & 7) > 0)
            {
                _bitIndex = (_bitIndex & 0x7ffffff8) + 8;
            }
        }

        private static uint GetValueFromBits(ReadOnlySpan<byte> source, int numberOfBits)
        {
            uint value = 0;

            while (numberOfBits > 0)
            {
                int bytePosition = _bitIndex & 7;
                int bitsLeftInByte = 8 - bytePosition;

                if (bytePosition == 0)
                {
                    _currentByte = source.ReadByte();
                }

                int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

                value = (value << bitsToRead) | (((uint)_currentByte >> bytePosition) & ((1u << bitsToRead) - 1u));

                _bitIndex += bitsToRead;
                numberOfBits -= bitsToRead;
            }

            return value;
        }

        private static ReadOnlySpan<byte> ReadBlob(ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            if (numberOfBits < 33)
                return ReadAlignedBytes(source, (int)ReadBits(source, numberOfBits));
            else
                return ReadAlignedBytes(source, (int)ReadULongBits(source, numberOfBits));
        }

        private static ReadOnlySpan<byte> ReadAlignedBytes(ReadOnlySpan<byte> source, int numberOfBytes)
        {
            return ReadBytes(source, numberOfBytes);
        }

        private static ulong GetULongValueFromBits(ReadOnlySpan<byte> source, int numberOfBits)
        {
            ulong value = 0;

            while (numberOfBits > 0)
            {
                int bytePosition = _bitIndex & 7;
                int bitsLeftInByte = 8 - bytePosition;

                if (bytePosition == 0)
                {
                    _currentByte = source.ReadByte();
                }

                int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

                value = (value << bitsToRead) | (((uint)_currentByte >> bytePosition) & ((1u << bitsToRead) - 1u));
                _bitIndex += bitsToRead;
                numberOfBits -= bitsToRead;
            }

            return value;
        }

        private static long GetLongValueFromBits(ReadOnlySpan<byte> source, int numberOfBits)
        {
            long value = 0;

            while (numberOfBits > 0)
            {
                int bytePosition = _bitIndex & 7;
                int bitsLeftInByte = 8 - bytePosition;

                if (bytePosition == 0)
                {
                    _currentByte = source.ReadByte();
                }

                int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

                value = (value << bitsToRead) | (((uint)_currentByte >> bytePosition) & ((1u << bitsToRead) - 1u));
                _bitIndex += bitsToRead;
                numberOfBits -= bitsToRead;
            }

            return value;
        }
    }
}
