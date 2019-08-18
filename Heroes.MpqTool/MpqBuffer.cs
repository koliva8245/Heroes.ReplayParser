﻿using System;
using System.Buffers.Binary;
using System.Text;

namespace Heroes.MpqTool
{
    public class MpqBuffer
    {
        private int _bitIndex; // current bit position in the current byte

        private ReadOnlyMemory<byte> _currentByte;

        public MpqBuffer()
        {
            Index = 0;
        }

        public MpqBuffer(ReadOnlyMemory<byte> buffer)
        {
            Buffer = buffer;
            Index = 0;
        }

        public ReadOnlyMemory<byte> Buffer { get; set; }

        /// <summary>
        /// Gets or sets the current position in the buffer.
        /// </summary>
        public int Index { get; set; }

        public bool IsEndOfBuffer => Buffer.Length == Index;

        public virtual int Length => Buffer.Length;

        /// <summary>
        /// Reads a signed integer of variable length.
        /// </summary>
        /// <returns></returns>
        public static long ReadVInt(ReadOnlySpan<byte> bytes)
        {
            int negative = bytes[0] & 1;
            long result = (bytes[0] >> 1) & 0x3f;
            int bits = 6;

            for (int i = 1; i < bytes.Length && (bytes[i - 1] & 0x80) != 0; i++)
            {
                result |= ((long)bytes[i] & 0x7f) << bits;
                bits += 7;
            }

            return negative < 0 ? -negative : result;
        }

        /// <summary>
        /// Reads a signed integer of variable length.
        /// </summary>
        /// <returns></returns>
        public static long ReadVInt(ReadOnlySpan<byte> bytes, out int size)
        {
            int negative = bytes[0] & 1;
            long result = (bytes[0] >> 1) & 0x3f;
            int bits = 6;

            for (int i = 1; i < bytes.Length && (bytes[i - 1] & 0x80) != 0; i++)
            {
                result |= ((long)bytes[i] & 0x7f) << bits;
                bits += 7;
            }

            size = (bits / 7) + 1;

            return negative < 0 ? -negative : result;
        }

        /// <summary>
        /// Reads one byte from the buffer.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyMemory<byte> ReadByte()
        {
            ByteAlign();

            ReadOnlyMemory<byte> value = Buffer.Slice(Index, 1);
            Index++;

            return value;
        }

        /// <summary>
        /// Reads a number of bytes from the buffer.
        /// </summary>
        /// <param name="count">The amount to read.</param>
        /// <returns></returns>
        public ReadOnlyMemory<byte> ReadBytes(int count)
        {
            ByteAlign();

            ReadOnlyMemory<byte> value = Buffer.Slice(Index, count);
            Index += count;

            return value;
        }

        /// <summary>
        /// Reads 2 bytes from the buffer and returns an 16-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public ushort ReadUInt16()
        {
            ByteAlign();

            ushort value = BinaryPrimitives.ReadUInt16LittleEndian(Buffer.Span.Slice(Index, 2));
            Index += 2;

            return value;
        }

        /// <summary>
        /// Reads 2 bytes from the buffer and returns a 16-bit signed integer.
        /// </summary>
        /// <returns></returns>
        public short ReadInt16()
        {
            ByteAlign();

            short value = BinaryPrimitives.ReadInt16LittleEndian(Buffer.Span.Slice(Index, 2));
            Index += 2;

            return value;
        }

        /// <summary>
        /// Reads 4 bytes from the buffer and returns a 32-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            ByteAlign();

            uint value = BinaryPrimitives.ReadUInt32LittleEndian(Buffer.Span.Slice(Index, 4));
            Index += 4;

            return value;
        }

        /// <summary>
        /// Reads 4 bytes from the buffer and returns a 32-bit signed integer.
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            ByteAlign();

            int value = BinaryPrimitives.ReadInt32LittleEndian(Buffer.Span.Slice(Index, 4));
            Index += 4;

            return value;
        }

        /// <summary>
        /// Reads 8 bytes from the buffer and returns a 64-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            ByteAlign();

            ulong value = BinaryPrimitives.ReadUInt64LittleEndian(Buffer.Span.Slice(Index, 8));
            Index += 8;

            return value;
        }

        /// <summary>
        /// Reads 8 bytes from the buffer and returns a 64-bit signed integer.
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            ByteAlign();

            long value = BinaryPrimitives.ReadInt64LittleEndian(Buffer.Span.Slice(Index, 8));
            Index += 8;

            return value;
        }

        /// <summary>
        /// Reads a signed integer of variable length.
        /// </summary>
        /// <returns></returns>
        public long ReadVInt()
        {
            ReadOnlySpan<byte> dataByte = ReadByte().Span;
            int negative = dataByte[0] & 1;
            long result = (dataByte[0] >> 1) & 0x3f;
            int bits = 6;

            while ((dataByte[0] & 0x80) != 0)
            {
                dataByte = ReadByte().Span;
                result |= ((long)dataByte[0] & 0x7f) << bits;
                bits += 7;
            }

            return negative < 0 ? -negative : result;
        }

        /// <summary>
        /// Returns the number of bytes read for a vInt.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyMemory<byte> ReadBytesForVInt()
        {
            int count = 1;

            ReadOnlySpan<byte> dataByte = ReadByte().Span;
            long result = (dataByte[0] >> 1) & 0x3f;
            int bits = 6;

            while ((dataByte[0] & 0x80) != 0)
            {
                count++;
                dataByte = ReadByte().Span;
                result |= ((long)dataByte[0] & 0x7f) << bits;
                bits += 7;
            }

            Index -= count;

            return ReadBytes(count);
        }

        /// <summary>
        /// Reads a line of characters.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyMemory<char> ReadLineAsMemory()
        {
            int startIndex = Index;
            do
            {
                ReadOnlySpan<byte> charByte = Buffer.Span.Slice(Index, 1);

                // \n - UNIX   \r\n - DOS   \r - Mac
                if (charByte[0] == 10 || charByte[0] == 13)
                {
                    Memory<char> data = new char[Index - startIndex];

                    Encoding.UTF8.GetChars(Buffer.Span.Slice(startIndex, Index - startIndex), data.Span);

                    // if it's a \r, check one ahead for a \n
                    if (charByte[0] == 13 && Index < Length)
                    {
                        ReadOnlySpan<byte> nByte = Buffer.Span.Slice(Index + 1, 1);
                        if (nByte[0] == 10)
                        {
                           Index++;
                        }
                    }

                    Index++;

                    return data;
                }

                Index++;
            } while (!IsEndOfBuffer);

            return null;
        }

        /// <summary>
        /// Reads up to 32 bits from the buffer and returns a 32-bit unsigned integer.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public uint ReadBits(int numberOfBits)
        {
            if (numberOfBits > 32)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 33");
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return GetValueFromBits(numberOfBits);
        }

        /// <summary>
        /// Reads up to 64 bits from the buffer and returns a 64-bit unsigned integer.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public ulong ReadULongBits(int numberOfBits)
        {
            if (numberOfBits > 64)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 65");
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return GetLongValueFromBits(numberOfBits);
        }

        /// <summary>
        /// Reads a blob given the number of bits.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public ReadOnlyMemory<byte> ReadBlob(int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            if (numberOfBits < 33)
                return ReadAlignedBytes((int)ReadBits(numberOfBits));
            else
                return ReadAlignedBytes((int)ReadULongBits(numberOfBits));
        }

        private ReadOnlyMemory<byte> ReadAlignedBytes(int numberOfBytes)
        {
            return ReadBytes(numberOfBytes);
        }

        /// <summary>
        /// If not at the beginning of a byte, moves it to the start of the next byte.
        /// </summary>
        private void ByteAlign()
        {
            if ((_bitIndex & 7) > 0)
            {
                _bitIndex = (_bitIndex & 0x7ffffff8) + 8;
            }
        }

        private uint GetValueFromBits(int numberOfBits)
        {
            uint value = 0;

            while (numberOfBits > 0)
            {
                int bytePosition = _bitIndex & 7;
                int bitsLeftInByte = 8 - bytePosition;

                if (bytePosition == 0)
                {
                    _currentByte = ReadByte();
                }

                int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

                value = (value << bitsToRead) | (((uint)_currentByte.Span[0] >> bytePosition) & ((1u << bitsToRead) - 1u));
                _bitIndex += bitsToRead;
                numberOfBits -= bitsToRead;
            }

            return value;
        }

        private ulong GetLongValueFromBits(int numberOfBits)
        {
            ulong value = 0;

            while (numberOfBits > 0)
            {
                int bytePosition = _bitIndex & 7;
                int bitsLeftInByte = 8 - bytePosition;

                if (bytePosition == 0)
                {
                    _currentByte = ReadByte();
                }

                int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

                value = (value << bitsToRead) | (((uint)_currentByte.Span[0] >> bytePosition) & ((1u << bitsToRead) - 1u));
                _bitIndex += bitsToRead;
                numberOfBits -= bitsToRead;
            }

            return value;
        }
    }
}
