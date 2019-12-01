using System;

namespace Heroes.MpqToolV2
{
    [Flags]
    public enum MpqFileFlags : uint
    {
        CompressedPK = 0x100, // AKA Imploded
        CompressedMulti = 0x200,
        Compressed = 0xff00,
        Encrypted = 0x10000,
        BlockOffsetAdjustedKey = 0x020000, // AKA FixSeed
        SingleUnit = 0x1000000,
        FileHasMetadata = 0x04000000, // Appears in WoW 1.10 or newer.  Indicates the file has associated metadata.
        Exists = 0x80000000,
    }
}
