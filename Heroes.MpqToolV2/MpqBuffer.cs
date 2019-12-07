using System;
using System.Buffers.Binary;
using System.Text;

namespace Heroes.MpqToolV2
{
    public class MpqBuffer
    {
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

        /// <summary>
        /// Gets the value indicating if the index is at the end of the buffer.
        /// </summary>
        public bool IsEndOfBuffer => Buffer.Length == Index;

        /// <summary>
        /// Gets the size of the buffer.
        /// </summary>
        public virtual int Length => Buffer.Length;

        /// <summary>
        /// Reads one byte from the buffer.
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            byte value = Buffer.Span[Index];
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
            long value = BinaryPrimitives.ReadInt64LittleEndian(Buffer.Span.Slice(Index, 8));
            Index += 8;

            return value;
        }

        /// <summary>
        /// Reads a line of characters.
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            int startIndex = Index;
            ReadOnlySpan<byte> dataSpan = Buffer.Span;

            do
            {
                byte charByte = dataSpan[Index];

                // \n - UNIX   \r\n - DOS   \r - Mac
                if (charByte == 10 || charByte == 13)
                {
                    Span<char> data = stackalloc char[Index - startIndex];

                    Encoding.UTF8.GetChars(dataSpan.Slice(startIndex, Index - startIndex), data);

                    // if it's a \r, check one ahead for a \n
                    if (charByte == 13 && Index < Length)
                    {
                        byte nByte = dataSpan[Index + 1];
                        if (nByte == 10)
                        {
                            Index++;
                        }
                    }

                    Index++;

                    return data.ToString();
                }

                Index++;
            } while (!IsEndOfBuffer);

            throw new IndexOutOfRangeException("Reached the end of the buffer");
        }
    }
}
