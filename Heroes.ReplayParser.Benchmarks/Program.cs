using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Heroes.MpqToolV2;
using Heroes.ReplayParser.MpqFiles;
using System.IO;

namespace Heroes.ReplayParser.Benchmarks
{
    [RankColumn, MemoryDiagnoser]
    public class ParseStormReplayBenchmark
    {
        private readonly string _fileName = "HanamuraTemple1.StormReplay";

        //private MpqMemory mpqMemory;

        public ParseStormReplayBenchmark()
        {
            //Reader.Index = 0;
        }


        //public void Setup()
        //{
        //    //using MpqArchive mpqArchive = new MpqArchive(File.Open(@"F:\Battlefield of Eternity1.StormReplay", FileMode.Open, FileAccess.Read));
        //    //mpqArchive.AddListfileFileNames();
        //    //mpqMemory = mpqArchive.OpenFile("replay.details");
        //    //mpqMemory.Index = 0;
        //    // bytes = DataParser.GetMpqFile(archive, "replay.details");
        // //   Reader.Index = 0;

        //}

        //public void CleanUp()
        //{
        //  //  Reader.Index = 0;
        //}

        [Benchmark]
        public void MpqToolV2()
        {
            //Reader.Index = 0;
            //Reader.BitIndex = 0;
            var a = StormReplayParser.Parse(@"F:\Battlefield of Eternity1.StormReplay");
          //  BitReader.ResetIndex();
           // BitReader.EndianType = EndianType.LittleEndian;
            //using MpqArchive mpqArchive = new MpqArchive(File.Open(@"F:\Battlefield of Eternity1.StormReplay", FileMode.Open, FileAccess.Read));
            //mpqArchive.AddListfileFileNames();
            //MpqMemory mpqMemory = mpqArchive.OpenFile("replay.details");

            //mpqMemory.Index = 0;
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();
            //mpqMemory.ReadInt32();

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

        //static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
    }
}
