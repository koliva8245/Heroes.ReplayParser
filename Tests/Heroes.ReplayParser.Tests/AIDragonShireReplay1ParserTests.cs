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
    public class AIDragonShireReplay1ParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly StormReplay _stormReplay;
        private readonly StormReplayParseResult _result;

        public AIDragonShireReplay1ParserTests()
        {
            _stormReplay = StormReplayParser.Parse(Path.Combine(_replaysFolder, "AIDragonShire1_75589.StormR"), out _result);
        }

        [TestMethod]
        public void ParseResult()
        {
            Assert.AreEqual(StormReplayParseResult.Success, _result);
        }

        [TestMethod]
        public void StormReplayHeaderTest()
        {
            Assert.AreEqual(2, _stormReplay.ReplayVersion.Major);
            Assert.AreEqual(47, _stormReplay.ReplayVersion.Minor);
            Assert.AreEqual(0, _stormReplay.ReplayVersion.Revision);
            Assert.AreEqual(75589, _stormReplay.ReplayVersion.Build);
            Assert.AreEqual(75589, _stormReplay.ReplayVersion.BaseBuild);

            Assert.AreEqual(75589, _stormReplay.ReplayBuild);
            Assert.AreEqual(13231, _stormReplay.ElapsedGamesLoops);
        }

        [TestMethod]
        public void StormReplayDetailsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("lavakill", player0.Name);
            Assert.AreEqual(1, player0.ToonHandle.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Blue, player0.Team);
            Assert.IsTrue(player0.IsWinner);
            Assert.AreEqual("Qhira", player0.PlayerHero.HeroName);

            StormPlayer player9 = players[9];

            Assert.AreEqual("Player 10", player9.Name);
            Assert.AreEqual(0, player9.ToonHandle.Region);
            Assert.AreEqual(0, player9.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Red, player9.Team);
            Assert.IsFalse(player9.IsWinner);
            Assert.AreEqual("Valeera", player9.PlayerHero.HeroName);

            Assert.AreEqual("Dragon Shire", _stormReplay.MapInfo.MapName);
            Assert.AreEqual(637010888527768698, _stormReplay.Timestamp.Ticks);

            Assert.IsFalse(_stormReplay.HasObservers);
            Assert.IsTrue(_stormReplay.HasAI);
        }

        [TestMethod]
        public void StormReplayAttributeEventsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player = players[9];

            Assert.AreEqual("5v5", _stormReplay.TeamSize);
            Assert.AreEqual(PlayerDifficulty.Veteran, player.PlayerDifficulty);
            Assert.AreEqual(GameSpeed.Faster, _stormReplay.GameSpeed);
            Assert.AreEqual(GameMode.Cooperative, _stormReplay.GameMode);
            Assert.AreEqual(string.Empty, player.PlayerHero.HeroAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.SkinAndSkinTintAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.MountAndMountTintAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.BannerAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.SprayAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.VoiceLineAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.AnnouncerPackAttributeId);
            Assert.AreEqual(1, player.PlayerHero.HeroLevel);

            List<string?> ban0List = _stormReplay.GetTeamBans(StormTeam.Blue).ToList();
            List<string?> ban1List = _stormReplay.GetTeamBans(StormTeam.Red).ToList();

            Assert.AreEqual(string.Empty, ban0List[1]);
            Assert.AreEqual(string.Empty, ban1List[1]);
        }

        [TestMethod]
        public void GetDraftOrderTest()
        {
            var draft = _stormReplay.GetDraftOrder().ToList();

            Assert.AreEqual(0, draft.Count);
        }

        [TestMethod]
        public void GetTeamLevelsTest()
        {
            List<TeamLevel> levelsBlue = _stormReplay.GetTeamLevels(StormTeam.Blue).ToList();
            List<TeamLevel> levelsRed = _stormReplay.GetTeamLevels(StormTeam.Red).ToList();

            Assert.AreEqual(15, levelsBlue.Count);
            Assert.AreEqual(15, levelsRed.Count);

            Assert.AreEqual(1, levelsBlue[0].Level);
            Assert.AreEqual(new TimeSpan(0, 0, 3), levelsBlue[0].Time);

            Assert.AreEqual(8, levelsBlue[7].Level);
            Assert.AreEqual(new TimeSpan(0, 5, 39), levelsBlue[7].Time);

            Assert.AreEqual(1, levelsRed[0].Level);
            Assert.AreEqual(new TimeSpan(0, 0, 3), levelsRed[0].Time);

            Assert.AreEqual(10, levelsRed[9].Level);
            Assert.AreEqual(new TimeSpan(0, 7, 15), levelsRed[9].Time);

        }
    }
}
