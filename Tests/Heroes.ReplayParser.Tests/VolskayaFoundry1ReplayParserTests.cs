using Heroes.ReplayParser.Player;
using Heroes.ReplayParser.Replay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Heroes.ReplayParser.Tests
{
    [TestClass]
    public class VolskayaFoundry1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly StormReplay _stormReplay;
        private readonly StormReplayParseResult _result;

        public VolskayaFoundry1ReplayParserTests()
        {
            _stormReplay = StormReplayParser.Parse(Path.Combine(_replaysFolder, "VolskayaFoundry1_77548.StormR"), out _result);
        }

        [TestMethod]
        public void ParseResult()
        {
            Assert.AreEqual(StormReplayParseResult.Success, _result);
        }

        [TestMethod]
        public void StormReplayHeaderTests()
        {
            Assert.AreEqual(2, _stormReplay.ReplayVersion.Major);
            Assert.AreEqual(49, _stormReplay.ReplayVersion.Minor);
            Assert.AreEqual(0, _stormReplay.ReplayVersion.Revision);
            Assert.AreEqual(77548, _stormReplay.ReplayVersion.Build);
            Assert.AreEqual(77548, _stormReplay.ReplayVersion.BaseBuild);

            Assert.AreEqual(77548, _stormReplay.ReplayBuild);
            Assert.AreEqual(17919, _stormReplay.ElapsedGamesLoops);
            Assert.AreEqual(new TimeSpan(0, 0, 18, 39, 0), _stormReplay.ReplayLength);
        }

        [TestMethod]
        public void StormReplayDetailsTests()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("Steph", player0.Name);
            Assert.AreEqual(1, player0.ToonHandle.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Blue, player0.Team);
            Assert.IsFalse(player0.IsWinner);
            Assert.AreEqual("Greymane", player0.PlayerHero.HeroName);

            StormPlayer player1 = players[9];

            Assert.AreEqual("Pinzs", player1.Name);
            Assert.AreEqual(1, player1.ToonHandle.Region);
            Assert.AreEqual(1, player1.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Red, player1.Team);
            Assert.IsTrue(player1.IsWinner);
            Assert.AreEqual("Rehgar", player1.PlayerHero.HeroName);

            Assert.AreEqual("Volskaya Foundry", _stormReplay.MapInfo.MapName);
            Assert.AreEqual(637120547862521860, _stormReplay.Timestamp.Ticks);

            Assert.IsFalse(_stormReplay.HasAI);
            Assert.IsFalse(_stormReplay.HasObservers);

            Assert.AreEqual(0, _stormReplay.StormObservers.ToList().Count);
            Assert.AreEqual(0, _stormReplay.PlayersObserversCount);
        }

        [TestMethod]
        public void StormReplayInitDataTests()
        {
            Assert.AreEqual(1102687070, _stormReplay.RandomValue);
            Assert.AreEqual(GameMode.StormLeague, _stormReplay.GameMode);

            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("GreymaneDoctorVar1", player0.PlayerLoadout.SkinAndSkinTint);
            Assert.AreEqual("MountCloud", player0.PlayerLoadout.MountAndMountTint);
            Assert.IsFalse(player0.IsSilenced);
            Assert.IsFalse(player0.IsVoiceSilenced);
            Assert.IsFalse(player0.IsBlizzardStaff);
            Assert.IsFalse(player0.HasActiveBoost);
            Assert.AreEqual("BannerOWDVaIconicRare", player0.PlayerLoadout.Banner);
            Assert.AreEqual("SprayStaticComicSweetChromie", player0.PlayerLoadout.Spray);
            Assert.AreEqual("DeckardA", player0.PlayerLoadout.AnnouncerPack);
            Assert.AreEqual("GreymaneBase_VoiceLine04", player0.PlayerLoadout.VoiceLine);
            Assert.AreEqual(24, player0.HeroMasteryTiers.Count);
            Assert.AreEqual("Auri", player0.HeroMasteryTiers[2].HeroAttributeId);
            Assert.AreEqual(1, player0.HeroMasteryTiers[2].TierLevel);
        }

        [TestMethod]
        public void StormReplayAttributeEvents()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player = players[9];

            Assert.AreEqual("5v5", _stormReplay.TeamSize);
            Assert.AreEqual(PlayerDifficulty.Elite, player.PlayerDifficulty);
            Assert.AreEqual(GameSpeed.Faster, _stormReplay.GameSpeed);
            Assert.AreEqual(GameMode.StormLeague, _stormReplay.GameMode);
            Assert.AreEqual("Rehg", player.PlayerHero.HeroAttributeId);
            Assert.AreEqual("Reh8", player.PlayerLoadout.SkinAndSkinTintAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.MountAndMountTintAttributeId);
            Assert.AreEqual("BN03", player.PlayerLoadout.BannerAttributeId);
            Assert.AreEqual("SY3K", player.PlayerLoadout.SprayAttributeId);
            Assert.AreEqual("RE05", player.PlayerLoadout.VoiceLineAttributeId);
            Assert.AreEqual("DEA0", player.PlayerLoadout.AnnouncerPackAttributeId);
            Assert.AreEqual(20, player.PlayerHero.HeroLevel);

            List<string?> ban0List = _stormReplay.GetTeamBans(StormTeam.Blue).ToList();
            List<string?> ban1List = _stormReplay.GetTeamBans(StormTeam.Red).ToList();

            Assert.AreEqual("Garr", ban0List[1]);
            Assert.AreEqual("DEAT", ban1List[1]);
        }
    }
}
