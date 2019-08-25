using Heroes.ReplayParser;
using Heroes.ReplayParser.Replay;

namespace Heroes.Console
{
    class Program
    {
        static void Main(string[] args)
        {
           // MpqArchive mpqArchive = new MpqArchive("HanamuraTemple1.StormReplay");
            //mpqArchive.AddListfileFilenames();

            StormReplay stormReplay = StormReplayParser.Parse("HanamuraTemple1.StormReplay");

            System.Console.WriteLine("Done");
        }
    }
}
