using Heroes.MpqTool;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Heroes.ReplayParser
{
    internal class TrackerEventStructure
    {
        private readonly ReadOnlyMemory<byte> _dataType;
        private readonly ReadOnlyMemory<byte> _value;

        public TrackerEventStructure()
        {
        }

        public TrackerEventStructure(MpqBuffer mpqBuffer)
        {
            _dataType = mpqBuffer.ReadByte();

            switch (_dataType.Span[0])
            {
                case 0x00: // array
                    ArrayData = new TrackerEventStructure[mpqBuffer.ReadVInt()];
                    for (var i = 0; i < ArrayData.Length; i++)
                        ArrayData[i] = new TrackerEventStructure(mpqBuffer);
                    break;
                //case 0x01: // bitarray, weird alignment requirements - haven't seen it used yet so not spending time on it
                //    /*  bits = self.read_vint()
                //        data = self.read_bits(bits) */
                //    throw new NotImplementedException();
                case 0x02: // blob
                    _value = mpqBuffer.ReadBytes((int)mpqBuffer.ReadVInt());

                    break;
                //case 0x03: // choice
                //    choiceFlag = (int)read_vint(reader);
                //    choiceData = new TrackerEventStructure(reader);
                //    break;
                case 0x04: // optional
                    if (mpqBuffer.ReadByte().Span[0] != 0)
                        OptionalData = new TrackerEventStructure(mpqBuffer);
                    break;
                case 0x05: // struct
                    StructureByIndex = new Dictionary<int, TrackerEventStructure>();
                    int size = (int)mpqBuffer.ReadVInt();

                    for (int i = 0; i < size; i++)
                    {
                        StructureByIndex[(int)mpqBuffer.ReadVInt()] = new TrackerEventStructure(mpqBuffer);
                    }

                    break;
                case 0x06: // u8
                    _value = mpqBuffer.ReadByte();
                    break;
                case 0x07: // u32
                    _value = mpqBuffer.ReadBytes(4);
                    break;
                case 0x08: // u64
                    _value = mpqBuffer.ReadBytes(8);
                    break;
                case 0x09: // vint
                    _value = mpqBuffer.ReadBytesForVInt();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public Dictionary<int, TrackerEventStructure>? StructureByIndex { get; private set; } = null;
        public TrackerEventStructure? OptionalData { get; private set; } = null;
        public TrackerEventStructure? ChoiceData { get; private set; } = null;
        public TrackerEventStructure[]? ArrayData { get; private set; } = null;

        /// <summary>
        /// Gets the value in the current structure as a signed 32-bit integer.
        /// </summary>
        /// <returns></returns>
        public int GetValueAsInt32()
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
                0x07 => (int)BinaryPrimitives.ReadUInt32LittleEndian(_value.Span),
                0x08 => throw new ArithmeticException("Incorrect conversion. Use Int64 method instead."),
                0x09 => Get32IntFromVInt(),

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
                0x09 => MpqBuffer.ReadVInt(_value.Span).ToString(),

                _ => throw new NotImplementedException(),
            };
        }

        private int Get32IntFromVInt()
        {
            int value = (int)MpqBuffer.ReadVInt(_value.Span, out int size);
            if (size > 4)
                throw new ArithmeticException("Incorrect conversion. Use Int64 method instead.");

            return value;
        }

        private long Get64IntFromVInt()
        {
            long value = (long)MpqBuffer.ReadVInt(_value.Span, out int size);
            if (size < 5)
                throw new ArithmeticException("Incorrect conversion. Use Int32 method instead.");

            return value;
        }
    }
}
