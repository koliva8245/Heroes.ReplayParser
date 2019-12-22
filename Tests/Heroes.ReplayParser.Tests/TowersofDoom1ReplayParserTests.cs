using Heroes.ReplayParser.Player;
using Heroes.ReplayParser.Replay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Heroes.ReplayParser.Tests
{
    [TestClass]
    public class TowersofDoom1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly StormReplay _stormReplay;
        private readonly StormReplayParseResult _result;

        public TowersofDoom1ReplayParserTests()
        {
            _stormReplay = StormReplayParser.Parse(Path.Combine(_replaysFolder, "TowersofDoom1_39445.StormR"), out _result);
        }

        [TestMethod]
        public void ParseResult()
        {
               Assert.AreEqual(StormReplayParseResult.Success, _result);
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

        [TestMethod]
        public void StormReplayInitDataTests()
        {
            Assert.AreEqual(4013533878, _stormReplay.RandomValue);
            Assert.AreEqual(GameMode.QuickMatch, _stormReplay.GameMode);

            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual(string.Empty, player0.PlayerLoadout.SkinAndSkinTint);
            Assert.AreEqual("BattleBeastNexus", player0.PlayerLoadout.MountAndMountTint);
            Assert.IsFalse(player0.IsSilenced);
            Assert.IsFalse(player0.IsVoiceSilenced);
            Assert.IsFalse(player0.IsBlizzardStaff);
            Assert.IsFalse(player0.HasActiveBoost);
            Assert.AreEqual(string.Empty, player0.PlayerLoadout.Banner);
            Assert.AreEqual(string.Empty, player0.PlayerLoadout.Spray);
            Assert.AreEqual(string.Empty, player0.PlayerLoadout.AnnouncerPack);
            Assert.AreEqual(string.Empty, player0.PlayerLoadout.VoiceLine);
            Assert.AreEqual(0, player0.HeroMasteryTiers.Count);

            Assert.IsFalse(_stormReplay.HasObservers);
            Assert.IsFalse(_stormReplay.HasAI);
        }
    }
}
