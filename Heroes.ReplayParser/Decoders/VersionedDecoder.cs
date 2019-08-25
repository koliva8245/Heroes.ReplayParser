using Heroes.MpqTool;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heroes.ReplayParser.Decoders
{
    internal class VersionedDecoder : BitPackedBuffer
    {
        private readonly ReadOnlyMemory<byte> _dataType;
        private readonly ReadOnlyMemory<byte> _value;

        public VersionedDecoder(MpqBuffer mpqBuffer)
            : base(mpqBuffer)
        {
            _dataType = ReadByte();

            switch (_dataType.Span[0])
            {
                case 0x00: // array
                    ArrayData = new VersionedDecoder[ReadVInt()];
                    for (var i = 0; i < ArrayData.Length; i++)
                        ArrayData[i] = new VersionedDecoder(mpqBuffer);
                    break;
                case 0x01: // bitblob
                    throw new NotImplementedException();
                case 0x02: // blob
                    _value = ReadBytes((int)ReadVInt());

                    break;
                //case 0x03: // choice
                //    choiceFlag = (int)read_vint(reader);
                //    choiceData = new TrackerEventStructure(reader);
                //    break;
                case 0x04: // optional
                    if (ReadByte().Span[0] != 0)
                        OptionalData = new VersionedDecoder(mpqBuffer);
                    break;
                case 0x05: // struct
                    StructureByIndex = new Dictionary<int, VersionedDecoder>();
                    int size = (int)ReadVInt();

                    for (int i = 0; i < size; i++)
                    {
                        StructureByIndex[(int)ReadVInt()] = new VersionedDecoder(mpqBuffer);
                    }

                    break;
                case 0x06: // u8
                    _value = ReadByte();
                    break;
                case 0x07: // u32
                    _value = ReadBytes(4);
                    break;
                case 0x08: // u64
                    _value = ReadBytes(8);
                    break;
                case 0x09: // vint
                    _value = ReadBytesForVInt();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public Dictionary<int, VersionedDecoder>? StructureByIndex { get; private set; } = null;
        public VersionedDecoder? OptionalData { get; private set; } = null;
        public VersionedDecoder? ChoiceData { get; private set; } = null;
        public VersionedDecoder[]? ArrayData { get; private set; } = null;

        /// <summary>
        /// Gets the value in the current structure as a signed 32-bit unsigned integer.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArithmeticException"></exception>
        /// <returns></returns>
        public uint GetValueAsUInt32()
        {
            return _dataType.Span[0] switch
            {
                0x00 => throw new InvalidOperationException("Invalid call, use ArrayData"),
                0x01 => throw new NotImplementedException(),
                0x02 => throw new InvalidOperationException("Invalid call, use GetValueAsString()"),
                0x03 => throw new NotImplementedException(),
                0x04 => throw new InvalidOperationException("Invalid call, use OptinalData"),
                0x05 => throw new InvalidOperationException("Invalid call, use StructureByIndex"),
                0x06 => _value.Span[0],
                0x07 => BinaryPrimitives.ReadUInt32LittleEndian(_value.Span),
                0x08 => throw new ArithmeticException("Incorrect conversion. Use Int64 method instead."),
                0x09 => Get32UIntFromVInt(),

                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Gets the value in the current structure as a unsigned 64-bit integer.
        /// </summary>
        /// <returns></returns>
        public ulong GetValueAsUInt64()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value in the current structure as a signed 64-bit integer.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArithmeticException"></exception>
        /// <returns></returns>
        public long GetValueAsInt64()
        {
            return _dataType.Span[0] switch
            {
                0x00 => throw new InvalidOperationException("Invalid call, use ArrayData"),
                0x01 => throw new NotImplementedException(),
                0x02 => throw new InvalidOperationException("Invalid call, use GetValueAsString()"),
                0x03 => throw new NotImplementedException(),
                0x04 => throw new InvalidOperationException("Invalid call, use OptinalData"),
                0x05 => throw new InvalidOperationException("Invalid call, use StructureByIndex"),
                0x06 => throw new ArithmeticException("Incorrect conversion. Use Int32 method instead."),
                0x07 => throw new ArithmeticException("Incorrect conversion. Use Int32 method instead."),
                0x08 => (long)BinaryPrimitives.ReadUInt64LittleEndian(_value.Span),
                0x09 => Get64IntFromVInt(),

                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Gets the value in the current structure as a string.
        /// </summary>
        /// <returns></returns>
        public string GetValueAsString() => !_value.IsEmpty ? Encoding.UTF8.GetString(_value.Span) : string.Empty;

        public override string? ToString()
        {
            return _dataType.Span[0] switch
            {
                0x00 => ArrayData != null ? $"[{string.Join(", ", ArrayData.Select(i => i?.ToString()))}]" : null,
                0x02 => @$"""{Encoding.UTF8.GetString(_value.Span)}""",
                0x04 => OptionalData?.ToString(),
                0x05 => StructureByIndex != null ? $"{{{string.Join(", ", StructureByIndex.Values.Select(i => i?.ToString()))}}}" : null,
                0x06 => _value.Span[0].ToString(),
                0x07 => BinaryPrimitives.ReadUInt32LittleEndian(_value.Span).ToString(),
                0x08 => BinaryPrimitives.ReadUInt64LittleEndian(_value.Span).ToString(),
                0x09 => ReadVInt(_value.Span).ToString(),

                _ => throw new NotImplementedException(),
            };
        }

        private uint Get32UIntFromVInt()
        {
            uint value = (uint)ReadVInt(_value.Span, out int size);
            if (size > 4)
                throw new ArithmeticException("Incorrect conversion. Use Int64 method instead.");

            return value;
        }

        private long Get64IntFromVInt()
        {
            long value = ReadVInt(_value.Span, out int size);
            if (size < 5)
                throw new ArithmeticException("Incorrect conversion. Use Int32 method instead.");

            return value;
        }
    }
}
