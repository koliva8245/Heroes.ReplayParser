using System;
using System.IO;

namespace Heroes.MpqToolV2
{
    internal class MpqHash
    {
        public static readonly uint Size = 16;

        private readonly BinaryReader? _binaryReader;

        public MpqHash()
        {
        }

        public MpqHash(BinaryReader binaryReader)
        {
            _binaryReader = binaryReader ?? throw new ArgumentNullException(nameof(binaryReader));

            Name1 = _binaryReader.ReadUInt32();
            Name2 = _binaryReader.ReadUInt32();
            Locale = _binaryReader.ReadUInt32(); // Normally 0 or UInt32.MaxValue (0xffffffff)
            BlockIndex = _binaryReader.ReadUInt32();
        }

        public uint Name1 { get; private set; }
        public uint Name2 { get; private set; }
        public uint Locale { get; private set; }
        public uint BlockIndex { get; private set; }
    }
}
