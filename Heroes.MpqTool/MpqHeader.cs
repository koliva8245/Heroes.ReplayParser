using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Heroes.MpqTool
{
    public class MpqHeader
    {
        public static readonly uint MpqId = 0x1a51504d; // 441536589
        public static readonly uint Size = 32;

        public uint Id { get; private set; } // Signature.  Should be 0x1a51504d
        public uint DataOffset { get; private set; } // Offset of the first file.  AKA Header size
        public uint ArchiveSize { get; private set; }
        public ushort MpqVersion { get; private set; } // Most are 0.  Burning Crusade = 1
        public ushort BlockSize { get; private set; } // Size of file block is 0x200 << BlockSize
        public uint HashTablePos { get; private set; }
        public uint BlockTablePos { get; private set; }
        public uint HashTableSize { get; private set; }
        public uint BlockTableSize { get; private set; }

        // Version 1 fields
        // The extended block table is an array of Int16 - higher bits of the offests in the block table.
        public long ExtendedBlockTableOffset { get; private set; }
        public short HashTableOffsetHigh { get; private set; }
        public short BlockTableOffsetHigh { get; private set; }

        public static MpqHeader? FromBuffer(MpqBuffer mpqBuffer)
        {
            uint id = mpqBuffer.ReadUInt32();

            if (id != MpqId)
                return null;

            MpqHeader mpqHeader = new MpqHeader()
            {
                Id = id,
                DataOffset = mpqBuffer.ReadUInt32(),
                ArchiveSize = mpqBuffer.ReadUInt32(),
                MpqVersion = mpqBuffer.ReadUInt16(),
                BlockSize = mpqBuffer.ReadUInt16(),
                HashTablePos = mpqBuffer.ReadUInt32(),
                BlockTablePos = mpqBuffer.ReadUInt32(),
                HashTableSize = mpqBuffer.ReadUInt32(),
                BlockTableSize = mpqBuffer.ReadUInt32(),
            };

            if (mpqHeader.MpqVersion == 1)
            {
                mpqHeader.ExtendedBlockTableOffset = mpqBuffer.ReadInt64();
                mpqHeader.HashTableOffsetHigh = mpqBuffer.ReadInt16();
                mpqHeader.BlockTableOffsetHigh = mpqBuffer.ReadInt16();
            }

            return mpqHeader;
        }

        public void SetHeaderOffset(long headerOffset)
        {
            HashTablePos += (uint)headerOffset;
            BlockTablePos += (uint)headerOffset;

            if (DataOffset == 0x6d9e4b86) // A protected archive.  Seen in some custom wc3 maps.
                DataOffset = (uint)(Size + headerOffset);
        }
    }
}
