using System;
using System.IO;

namespace Heroes.MpqToolV2
{
    internal class MpqHeader
    {
        public static readonly uint MpqId = 0x1a51504d; // 441536589
        public static readonly uint Size = 32;

        private readonly BinaryReader _binaryReader;

        public MpqHeader(BinaryReader binaryReader)
        {
            _binaryReader = binaryReader ?? throw new ArgumentNullException(nameof(binaryReader));

            if (!LocateHeader())
                throw new MpqToolException("Could not locate the header");

            DataOffset = binaryReader.ReadUInt32();
            ArchiveSize = binaryReader.ReadUInt32();
            MpqVersion = binaryReader.ReadUInt16();
            BlockSize = binaryReader.ReadUInt16();
            HashTablePos = binaryReader.ReadUInt32();
            BlockTablePos = binaryReader.ReadUInt32();
            HashTableSize = binaryReader.ReadUInt32();
            BlockTableSize = binaryReader.ReadUInt32();

            if (MpqVersion == 1)
            {
                ExtendedBlockTableOffset = binaryReader.ReadInt64();
                HashTableOffsetHigh = binaryReader.ReadInt16();
                BlockTableOffsetHigh = binaryReader.ReadInt16();
            }

            HashTablePos += (uint)HeaderOffset;
            BlockTablePos += (uint)HeaderOffset;

            if (DataOffset == 0x6d9e4b86) // A protected archive.  Seen in some custom wc3 maps.
                DataOffset = (uint)(Size + HeaderOffset);
        }

        public uint DataOffset { get; private set; } // Offset of the first file.  AKA Header size
        public uint ArchiveSize { get; private set; }
        public ushort MpqVersion { get; private set; } // Most are 0.  Burning Crusade = 1
        public ushort BlockSize { get; private set; } // Size of file block is 0x200 << BlockSize
        public uint HashTablePos { get; private set; }
        public uint BlockTablePos { get; private set; }
        public uint HashTableSize { get; private set; }
        public uint BlockTableSize { get; private set; }

        // Version 1 fields
        // The extended block table is an array of Int16 - higher bits of the offsets in the block table.
        public long ExtendedBlockTableOffset { get; private set; }
        public short HashTableOffsetHigh { get; private set; }
        public short BlockTableOffsetHigh { get; private set; }

        public long HeaderOffset { get; private set; }

        public MpqMemory HeaderData { get; private set; }

        private bool LocateHeader()
        {
            Memory<byte> data = new byte[0x100];

            _binaryReader.BaseStream.Read(data.Span);

            HeaderData = new MpqMemory(data);

            for (long i = 0x200; i < _binaryReader.BaseStream.Length - Size; i += 0x200)
            {
                _binaryReader.BaseStream.Seek(i, SeekOrigin.Begin);

                uint id = _binaryReader.ReadUInt32();

                if (id == MpqId)
                {
                    HeaderOffset = i;

                    return true;
                }
            }

            return false;

            //for (long i = 0; i < _binaryReader.BaseStream.Length - Size; i += 0x200)
            //{
            //    _binaryReader.BaseStream.Seek(i, SeekOrigin.Begin);

            //    uint id = _binaryReader.ReadUInt32();

            //    if (id == MpqId)
            //    {
            //        HeaderOffset = i;

            //        return true;
            //    }
            //}

            //return false;
        }
    }
}
