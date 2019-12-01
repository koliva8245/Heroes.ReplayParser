using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Heroes.MpqToolV2;
using System.IO;

namespace Heroes.ReplayParser.Benchmarks
{
    [RankColumn, MemoryDiagnoser]
    public class ParseStormReplayBenchmark
    {
        private readonly string _fileName = "HanamuraTemple1.StormReplay";

        //private MpqMemory mpqMemory;

        //[GlobalSetup]
        //public void Setup()
        //{
        //    using MpqArchive mpqArchive = new MpqArchive(File.Open(@"F:\Battlefield of Eternity1.StormReplay", FileMode.Open, FileAccess.Read));
        //    mpqArchive.AddListfileFileNames();
        //    mpqMemory = mpqArchive.OpenFile("replay.details");
        //    //mpqMemory.Index = 0;
        //   // bytes = DataParser.GetMpqFile(archive, "replay.details");

        //}
        [Benchmark]
        public void MpqToolV2()
        {
            using MpqArchive mpqArchive = new MpqArchive(File.Open(@"F:\Battlefield of Eternity1.StormReplay", FileMode.Open, FileAccess.Read));
            mpqArchive.AddListfileFileNames();
            MpqMemory mpqMemory = mpqArchive.OpenFile("replay.details");

            mpqMemory.Index = 0;
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();
            mpqMemory.ReadInt32();

            //using MpqArchive mpqArchive = new MpqArchive(File.Open(@"F:\Battlefield of Eternity1.StormReplay", FileMode.Open, FileAccess.Read));
            //mpqArchive.AddListfileFileNames();
            //var a = mpqArchive.OpenFile("replay.details");
            //Heroes.MpqToolV2.MpqFile.Open(@"F:\Battlefield of Eternity1.StormReplay");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            _ = BenchmarkRunner.Run<ParseStormReplayBenchmark>();
        }
    }
}
