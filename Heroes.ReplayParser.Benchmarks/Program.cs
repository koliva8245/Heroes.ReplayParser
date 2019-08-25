using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Heroes.ReplayParser.Benchmarks
{
    [RankColumn]
    public class ParseStormReplayBenchmark
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
            _ = BenchmarkRunner.Run<ParseStormReplayBenchmark>();
        }
    }
}
