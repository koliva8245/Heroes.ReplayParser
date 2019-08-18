using Heroes.MpqTool;
using Heroes.ReplayParser.Replay;

namespace Heroes.ReplayParser.MpqFile
{
    internal interface IMpqParsable
    {
        public string FileName { get; }

        public void Parse(StormReplay stormReplay, MpqBuffer mpqBuffer);
    }
}
