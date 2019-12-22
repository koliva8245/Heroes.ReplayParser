using Heroes.ReplayParser.Replay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Heroes.ReplayParser.Tests
{
    [TestClass]
    public class TowersofDoom1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly StormReplay _stormReplay;

        public TowersofDoom1ReplayParserTests()
        {
            _stormReplay = StormReplayParser.Parse(Path.Combine(_replaysFolder, "TowersofDoom1_39445.StormR"));
        }

        [TestMethod]
        public void StormReplayHeaderTests()
        {
            Assert.AreEqual(0, _stormReplay.ReplayVersion.Major);
            Assert.AreEqual(15, _stormReplay.ReplayVersion.Minor);
            Assert.AreEqual(2, _stormReplay.ReplayVersion.Revision);
            Assert.AreEqual(39445, _stormReplay.ReplayVersion.Build);
            Assert.AreEqual(39445, _stormReplay.ReplayVersion.BaseBuild);

            Assert.AreEqual(39445, _stormReplay.ReplayBuild);
            Assert.AreEqual(18632, _stormReplay.ElapsedGamesLoops);
        }
    }
}
