﻿using Heroes.MpqToolV2;
using Heroes.ReplayParser;
using Heroes.ReplayParser.MpqFiles;
using Heroes.ReplayParser.Replay;
using System.IO;
using System.IO.Compression;

namespace Heroes.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //MpqArchive mpqArchive = new MpqArchive("HanamuraTemple1.StormReplay");
            //mpqArchive.AddListfileFilenames();

            StormReplay stormReplay = StormReplayParser.Parse(@"F:\Battlefield of Eternity1.StormReplay", out StormReplayParseResult _);

            //BitReader.EndianType = EndianType.LittleEndian;
            
            //StormReplay stormReplay2 = StormReplayParser.Parse(@"F:\Battlefield of Eternity1.StormReplay");

            //int a = stormReplay.ReplayVersion.Major;
            System.Console.WriteLine("Done");



            //MpqFile.Open(@"F:\Battlefield of Eternity1.StormReplay");

           // FileStream fileStream = new FileStream(@"F:\Battlefield of Eternity1.StormReplay", FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, false);

            //MpqArchive mpqArchive = new MpqArchive(fileStream);
           // mpqArchive.AddListfileFileNames();

           // var a = mpqArchive.OpenFile("replay.details");
            //a.ReadInt32();
        }
    }
}
