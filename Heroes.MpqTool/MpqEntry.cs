using System;
using System.IO;

namespace Heroes.MpqTool
{
    public class MpqEntry
    {
        public static readonly uint Size = 16;

        private readonly uint _fileOffset; // Relative to the header offset
        private ReadOnlyMemory<char> _fileName;

        public MpqEntry(MpqBuffer mpqBuffer, uint headerOffset)
        {
            _fileOffset = mpqBuffer.ReadUInt32();
            FilePosition = headerOffset + _fileOffset;
            CompressedSize = mpqBuffer.ReadUInt32();
            FileSize = mpqBuffer.ReadUInt32();
            Flags = (MpqFileFlags)mpqBuffer.ReadUInt32();
            EncryptionSeed = 0;
        }

        public uint CompressedSize { get; private set; }
        public uint FileSize { get; private set; }
        public MpqFileFlags Flags { get; internal set; }
        public uint EncryptionSeed { get; internal set; }
        public uint FilePosition { get; private set; } // Absolute position in the file

        public ReadOnlyMemory<char> FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                EncryptionSeed = CalculateEncryptionSeed();
            }
        }

        public bool IsEncrypted => (Flags & MpqFileFlags.Encrypted) != 0;

        public bool IsCompressed => (Flags & MpqFileFlags.Compressed) != 0;

        public bool Exists => Flags != 0;

        public bool IsSingleUnit => (Flags & MpqFileFlags.SingleUnit) != 0;

        // For debugging
        public int FlagsAsInt => (int)Flags;

        public override string ToString()
        {
            if (FileName.IsEmpty)
            {
                if (!Exists)
                    return "(Deleted file)";
                return string.Format("Unknown file @ {0}", FilePosition);
            }

            return FileName.ToString();
        }

        private uint CalculateEncryptionSeed()
        {
            if (FileName.IsEmpty) return 0;

            uint seed = MpqArchive.HashString(Path.GetFileName(FileName.Span), 0x300);
            if ((Flags & MpqFileFlags.BlockOffsetAdjustedKey) == MpqFileFlags.BlockOffsetAdjustedKey)
                seed = (seed + _fileOffset) ^ FileSize;
            return seed;
        }
    }
}
