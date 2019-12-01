using System;
using System.IO;

namespace Heroes.MpqToolV2
{
    public class MpqArchiveEntry
    {
        public static readonly uint Size = 16;

        private readonly BinaryReader _binaryReader;
        private readonly uint _fileOffset; // Relative to the header offset

        private string? _fileName;

        internal MpqArchiveEntry(BinaryReader binaryReader, uint headerOffset)
        {
            _binaryReader = binaryReader ?? throw new ArgumentNullException(nameof(binaryReader));

            _fileOffset = _binaryReader.ReadUInt32();
            FilePosition = headerOffset + _fileOffset;
            CompressedSize = _binaryReader.ReadUInt32();
            FileSize = _binaryReader.ReadUInt32();
            Flags = (MpqFileFlags)_binaryReader.ReadUInt32();
            EncryptionSeed = 0;
        }

        public uint CompressedSize { get; private set; }
        public uint FileSize { get; private set; }
        public MpqFileFlags Flags { get; internal set; }
        public uint EncryptionSeed { get; internal set; }
        public uint FilePosition { get; private set; } // Absolute position in the file

        public string? FileName
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
            if (FileName == null)
            {
                if (!Exists)
                    return "(Deleted file)";
                return string.Format("Unknown file @ {0}", FilePosition);
            }

            return FileName;
        }

        private uint CalculateEncryptionSeed()
        {
            if (FileName == null) return 0;

            uint seed = MpqArchive.HashString(Path.GetFileName(FileName), 0x300);
            if ((Flags & MpqFileFlags.BlockOffsetAdjustedKey) == MpqFileFlags.BlockOffsetAdjustedKey)
                seed = (seed + _fileOffset) ^ FileSize;
            return seed;
        }
    }
}
