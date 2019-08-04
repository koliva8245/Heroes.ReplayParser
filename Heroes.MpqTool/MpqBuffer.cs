using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace Heroes.MpqTool
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

        public bool IsEndOfBuffer => Buffer.Length == Index;

        public virtual int Length => Buffer.Length;

        public ReadOnlyMemory<byte> ReadBytes(int count)
        {
            ReadOnlyMemory<byte> value = Buffer.Slice(Index, count);
            Index += count;

            return value;
        }

        public virtual ushort ReadUInt16()
        {
            ushort value = BinaryPrimitives.ReadUInt16LittleEndian(Buffer.Span.Slice(Index, 2));
            Index += 2;

            return value;
        }

        public virtual short ReadInt16()
        {
            short value = BinaryPrimitives.ReadInt16LittleEndian(Buffer.Span.Slice(Index, 2));
            Index += 2;

            return value;
        }

        public virtual uint ReadUInt32()
        {
            uint value = BinaryPrimitives.ReadUInt32LittleEndian(Buffer.Span.Slice(Index, 4));
            Index += 4;

            return value;
        }

        public virtual int ReadInt32()
        {
            int value = BinaryPrimitives.ReadInt32LittleEndian(Buffer.Span.Slice(Index, 4));
            Index += 4;

            return value;
        }

        public virtual ulong ReadUInt64()
        {
            ulong value = BinaryPrimitives.ReadUInt64LittleEndian(Buffer.Span.Slice(Index, 8));
            Index += 8;

            return value;
        }

        public virtual long ReadInt64()
        {
            long value = BinaryPrimitives.ReadInt64LittleEndian(Buffer.Span.Slice(Index, 8));
            Index += 8;

            return value;
        }

        //public ReadOnlySpan<char> ReadStringAsSpan()
        //{

        //}

        public virtual ReadOnlyMemory<char> ReadStringAsMemory()
        {
            int startIndex = Index;
            do
            {
                ReadOnlySpan<byte> charByte = Buffer.Span.Slice(Index, 1);

                // \n - UNIX   \r\n - DOS   \r - Mac
                if (charByte[0] == 10 || charByte[0] == 13)
                {
                    Memory<char> data = new char[Index - startIndex];

                    int length = Encoding.UTF8.GetChars(Buffer.Span.Slice(startIndex, Index - startIndex), data.Span);

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
    }
}
