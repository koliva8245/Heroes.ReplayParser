using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Heroes.ReplayParser.Replay;

namespace Heroes.ReplayParser.Tests.Performance
{
    [RankColumn]
    public class StormReplayBenchmark
    {
        private readonly string _fileName = "HanamuraTemple1.StormReplay";

        [Benchmark]
        public void ParseStormReplay()
        {
            StormReplayParser.Parse(_fileName);
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<StormReplayBenchmark>();
        }
    }
}
