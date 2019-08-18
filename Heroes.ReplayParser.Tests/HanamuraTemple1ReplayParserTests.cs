using Heroes.ReplayParser.Player;
using Heroes.ReplayParser.Replay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Heroes.ReplayParser.Tests
{
    [TestClass]
    public class HanamuraTemple1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly StormReplay _stormReplay;

        public HanamuraTemple1ReplayParserTests()
        {
            _stormReplay = StormReplayParser.Parse(Path.Combine(_replaysFolder, "HanamuraTemple1_75132.StormReplay"));
        }

        [TestMethod]
        public void StormReplayHeaderTests()
        {
            Assert.AreEqual(2, _stormReplay.ReplayVersion.Major);
            Assert.AreEqual(46, _stormReplay.ReplayVersion.Minor);
            Assert.AreEqual(1, _stormReplay.ReplayVersion.Revision);
            Assert.AreEqual(75132, _stormReplay.ReplayVersion.Build);
            Assert.AreEqual(75132, _stormReplay.ReplayVersion.BaseBuild);

            Assert.AreEqual(75132, _stormReplay.ReplayBuild);
            Assert.AreEqual(21870, _stormReplay.ElapsedGamesLoops);
        }

        [TestMethod]
        public void StormReplayDetailsTests()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("crazealot", player0.Name);
            Assert.AreEqual(1, player0.ToonHandle.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(0, player0.Team);
            Assert.IsTrue(player0.IsWinner);
            Assert.AreEqual("Brightwing", player0.PlayerHero.HeroName);

            StormPlayer player1 = players[9];

            Assert.AreEqual("DumbleBore", player1.Name);
            Assert.AreEqual(1, player1.ToonHandle.Region);
            Assert.AreEqual(1, player1.ToonHandle.Realm);
            Assert.AreEqual(1, player1.Team);
            Assert.IsFalse(player1.IsWinner);
            Assert.AreEqual("Hanzo", player1.PlayerHero.HeroName);

            Assert.AreEqual("Hanamura Temple", _stormReplay.MapInfo.MapName);
            Assert.AreEqual(636997822244093849, _stormReplay.Timestamp.Ticks);
        }

        [TestMethod]
        public void StormReplayInitDataTests()
        {
            Assert.AreEqual(2143281452, _stormReplay.RandomValue);
            Assert.AreEqual(GameMode.QuickMatch, _stormReplay.GameMode);

            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("BrightwingLuxMonkeyWhite", player0.PlayerLoadout.SkinAndSkinTint);
            Assert.AreEqual(string.Empty, player0.PlayerLoadout.MountAndMountTint);
            Assert.IsFalse(player0.IsSilenced);
            Assert.IsFalse(player0.IsVoiceSilenced);
            Assert.IsFalse(player0.IsBlizzardStaff);
            Assert.IsTrue(player0.HasActiveBoost);
            Assert.AreEqual("BannerD3WitchDoctorRareVar2", player0.PlayerLoadout.Banner);
            Assert.AreEqual("SprayStaticWinterRewardReigndeer", player0.PlayerLoadout.Spray);
            Assert.AreEqual("MurkyA", player0.PlayerLoadout.AnnouncerPack);
            Assert.AreEqual("BrightwingMonkey_VoiceLine01", player0.PlayerLoadout.VoiceLine);
            Assert.AreEqual(22, player0.HeroMasteryTiers.Count);
            Assert.AreEqual("Arts", player0.HeroMasteryTiers[2].HeroAttributeId);
            Assert.AreEqual(3, player0.HeroMasteryTiers[2].TierLevel);
        }
    }
}
