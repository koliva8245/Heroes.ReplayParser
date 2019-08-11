using Heroes.MpqTool;
using Heroes.ReplayParser;
using System;

namespace Heroes.Console
{
    class Program
    {
        static void Main(string[] args)
        {
           // MpqArchive mpqArchive = new MpqArchive("HanamuraTemple1.StormReplay");
            //mpqArchive.AddListfileFilenames();

            StormReplay stormReplay = StormReplayParser.Parse("HanamuraTemple1.StormReplay");
        }
    }
}
