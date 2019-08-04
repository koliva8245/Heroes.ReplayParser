namespace Heroes.MpqTool
{
    public class MpqHash
    {
        public static readonly uint Size = 16;

        public MpqHash()
        {
        }

        public MpqHash(MpqBuffer mpqBuffer)
        {
            Name1 = mpqBuffer.ReadUInt32();
            Name2 = mpqBuffer.ReadUInt32();
            Locale = mpqBuffer.ReadUInt32(); // Normally 0 or UInt32.MaxValue (0xffffffff)
            BlockIndex = mpqBuffer.ReadUInt32();
        }

        public uint Name1 { get; private set; }
        public uint Name2 { get; private set; }
        public uint Locale { get; private set; }
        public uint BlockIndex { get; private set; }
    }
}
