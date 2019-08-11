using Heroes.ReplayParser.Player;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Heroes.ReplayParser.Tests
{
    [TestClass]
    public class CustomBattlefieldofEternity1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly StormReplay _stormReplay;

        public CustomBattlefieldofEternity1ReplayParserTests()
        {
            _stormReplay = StormReplayParser.Parse(Path.Combine(_replaysFolder, "CustomBattlefieldofEternity1_65006.StormReplay"));
        }

        [TestMethod]
        public void StormReplayHeaderTests()
        {
            Assert.AreEqual(2, _stormReplay.ReplayVersion.Major);
            Assert.AreEqual(32, _stormReplay.ReplayVersion.Minor);
            Assert.AreEqual(3, _stormReplay.ReplayVersion.Revision);
            Assert.AreEqual(65006, _stormReplay.ReplayVersion.Build);
            Assert.AreEqual(65006, _stormReplay.ReplayVersion.BaseBuild);

            Assert.AreEqual(65006, _stormReplay.ReplayBuild);
            Assert.AreEqual(19306, _stormReplay.ElapsedGamesLoops);
        }

        [TestMethod]
        public void StormReplayDetailsTests()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("AZTDoubt", player0.Name);
            Assert.AreEqual(1, player0.ToonHandle.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(0, player0.Team);
            Assert.IsFalse(player0.IsWinner);
            Assert.AreEqual("Greymane", player0.HeroName);

            StormPlayer player1 = players[9];

            Assert.AreEqual("FiZiX", player1.Name);
            Assert.AreEqual(1, player1.ToonHandle.Region);
            Assert.AreEqual(1, player1.ToonHandle.Realm);
            Assert.AreEqual(1, player1.Team);
            Assert.IsTrue(player1.IsWinner);
            Assert.AreEqual("Li-Ming", player1.HeroName);

            Assert.AreEqual("Battlefield of Eternity", _stormReplay.MapName);
            Assert.AreEqual(636619794857150779, _stormReplay.Timestamp.Ticks);
        }
    }
}
