using Heroes.MpqTool;
using System;
using System.Buffers.Binary;

namespace Heroes.ReplayParser.Decoders
{
    internal class BitPackedBuffer
    {
        private readonly EndianType _endianType;

        private int _bitIndex; // current bit position in the current byte
        private ReadOnlyMemory<byte> _currentByte;

        public BitPackedBuffer(MpqBuffer mpqBuffer, EndianType type = EndianType.BigEndian)
        {
            MpqBuffer = mpqBuffer;
            _endianType = type;
        }

        public bool IsEndofBuffer => MpqBuffer.IsEndOfBuffer;

        protected MpqBuffer MpqBuffer { get; }

        /// <summary>
        /// Reads a signed integer of variable length.
        /// </summary>
        /// <returns></returns>
        public long ReadVInt(ReadOnlySpan<byte> bytes)
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
        public long ReadVInt(ReadOnlySpan<byte> bytes, out int size)
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

            MpqBuffer.Index -= count;

            return ReadBytes(count);
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

            return _endianType == EndianType.BigEndian ? GetValueFromBits(numberOfBits) : BinaryPrimitives.ReverseEndianness(GetValueFromBits(numberOfBits));
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

            return _endianType == EndianType.BigEndian ? GetLongValueFromBits(numberOfBits) : BinaryPrimitives.ReverseEndianness(GetLongValueFromBits(numberOfBits));
        }

        /// <summary>
        /// Reads one aligned byte from the buffer.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyMemory<byte> ReadByte()
        {
            ByteAlign();

            return MpqBuffer.ReadByte();
        }

        /// <summary>
        /// Reads a number of aligned bytes from the buffer.
        /// </summary>
        /// <param name="count">The amount to read.</param>
        /// <returns></returns>
        public ReadOnlyMemory<byte> ReadBytes(int count)
        {
            ByteAlign();

            return MpqBuffer.ReadBytes(count);
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

        private ReadOnlyMemory<byte> ReadAlignedBytes(int numberOfBytes)
        {
            return ReadBytes(numberOfBytes);
        }
    }
}
