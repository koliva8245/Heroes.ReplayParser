using Heroes.ReplayParser.MessageEvent;
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
        public void StormReplayHeaderTest()
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
        public void StormReplayDetailsTest()
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
        public void StormReplayInitDataTest()
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
            Assert.AreEqual(24, player0.HeroMasteryTiersCount);
            Assert.AreEqual("Auri", player0.HeroMasteryTiers.ToList()[2].HeroAttributeId);
            Assert.AreEqual(1, player0.HeroMasteryTiers.ToList()[2].TierLevel);
        }

        [TestMethod]
        public void StormReplayAttributeEventsTest()
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

        [TestMethod]
        public void DraftOrderTest()
        {
            var draft = _stormReplay.GetDraftOrder().ToList();

            Assert.AreEqual(16, draft.Count);

            Assert.AreEqual("Crusader", draft[0].HeroSelected);
            Assert.AreEqual(DraftPickType.Banned, draft[0].PickType);
            Assert.AreEqual(2, draft[0].SelectedPlayerSlotId);

            Assert.AreEqual("Auriel", draft[15].HeroSelected);
            Assert.AreEqual(DraftPickType.Picked, draft[15].PickType);
            Assert.AreEqual(2, draft[15].SelectedPlayerSlotId);
        }

        [TestMethod]
        public void TeamLevelsTest()
        {
            List<TeamLevel> levelsBlue = _stormReplay.GetTeamLevels(StormTeam.Blue).ToList();
            List<TeamLevel> levelsBlue2 = _stormReplay.GetTeamLevels(StormTeam.Blue).ToList();
            List<TeamLevel> levelsRed = _stormReplay.GetTeamLevels(StormTeam.Red).ToList();
            List<TeamLevel> levelsRed2 = _stormReplay.GetTeamLevels(StormTeam.Red).ToList();
            List<TeamLevel> levelsOther = _stormReplay.GetTeamLevels(StormTeam.Observer).ToList();

            Assert.AreEqual(19, levelsBlue.Count);
            Assert.AreEqual(19, levelsBlue2.Count);
            Assert.AreEqual(21, levelsRed.Count);
            Assert.AreEqual(21, levelsRed2.Count);
            Assert.AreEqual(0, levelsOther.Count);

            Assert.AreEqual(1, levelsBlue[0].Level);
            Assert.AreEqual(new TimeSpan(0, 0, 3), levelsBlue[0].Time);

            Assert.AreEqual(8, levelsBlue[7].Level);
            Assert.AreEqual(new TimeSpan(0, 5, 28), levelsBlue[7].Time);

            Assert.AreEqual(18, levelsBlue[17].Level);
            Assert.AreEqual(new TimeSpan(0, 15, 58), levelsBlue[17].Time);

            Assert.AreEqual(1, levelsRed[0].Level);
            Assert.AreEqual(new TimeSpan(0, 0, 3), levelsRed[0].Time);

            Assert.AreEqual(10, levelsRed[9].Level);
            Assert.AreEqual(new TimeSpan(0, 6, 9), levelsRed[9].Time);

            Assert.AreEqual(20, levelsRed[19].Level);
            Assert.AreEqual(new TimeSpan(0, 16, 33), levelsRed[19].Time);
        }

        [TestMethod]
        public void TeamsFinalLevelTest()
        {
            Assert.AreEqual(19, _stormReplay.GetTeamFinalLevel(StormTeam.Blue));
            Assert.AreEqual(21, _stormReplay.GetTeamFinalLevel(StormTeam.Red));
            Assert.AreEqual(0, _stormReplay.GetTeamFinalLevel(StormTeam.Observer));
        }

        [TestMethod]
        public void TeamXpBreakdownTest()
        {
            List<TeamXPBreakdown> xpBlue = _stormReplay.GetTeamXPBreakdown(StormTeam.Blue).ToList();
            List<TeamXPBreakdown> xpRed = _stormReplay.GetTeamXPBreakdown(StormTeam.Red).ToList();
            List<TeamXPBreakdown> xpOther = _stormReplay.GetTeamXPBreakdown(StormTeam.Observer).ToList();

            Assert.AreEqual(18, xpBlue.Count);
            Assert.AreEqual(18, xpRed.Count);
            Assert.AreEqual(0, xpOther.Count);

            TeamXPBreakdown blue = xpBlue[0];

            Assert.AreEqual(0, blue.HeroXP);
            Assert.AreEqual(1, blue.Level);
            Assert.AreEqual(0, blue.CreepXP);
            Assert.AreEqual(1200, blue.MinionXP);
            Assert.AreEqual(575, blue.PassiveXP);
            Assert.AreEqual(0, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(0, 1, 38), blue.Time);
            Assert.AreEqual(1775, blue.TotalXP);

            blue = xpBlue[13];
            Assert.AreEqual(5470, blue.HeroXP);
            Assert.AreEqual(16, blue.Level);
            Assert.AreEqual(3773, blue.CreepXP);
            Assert.AreEqual(25754, blue.MinionXP);
            Assert.AreEqual(18515, blue.PassiveXP);
            Assert.AreEqual(375, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(0, 14, 38), blue.Time);
            Assert.AreEqual(53887, blue.TotalXP);

            blue = xpBlue[17];
            Assert.AreEqual(10164, blue.HeroXP);
            Assert.AreEqual(19, blue.Level);
            Assert.AreEqual(4121, blue.CreepXP);
            Assert.AreEqual(32032, blue.MinionXP);
            Assert.AreEqual(23621, blue.PassiveXP);
            Assert.AreEqual(375, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(0, 18, 27), blue.Time);
            Assert.AreEqual(70313, blue.TotalXP);

            TeamXPBreakdown red = xpRed[0];

            Assert.AreEqual(592, red.HeroXP);
            Assert.AreEqual(2, red.Level);
            Assert.AreEqual(0, red.CreepXP);
            Assert.AreEqual(1360, red.MinionXP);
            Assert.AreEqual(575, red.PassiveXP);
            Assert.AreEqual(0, red.StructureXP);
            Assert.AreEqual(new TimeSpan(0, 1, 38), red.Time);
            Assert.AreEqual(2527, red.TotalXP);

            red = xpRed[13];
            Assert.AreEqual(6596, red.HeroXP);
            Assert.AreEqual(18, red.Level);
            Assert.AreEqual(3661, red.CreepXP);
            Assert.AreEqual(31496, red.MinionXP);
            Assert.AreEqual(21288, red.PassiveXP);
            Assert.AreEqual(1425, red.StructureXP);
            Assert.AreEqual(new TimeSpan(0, 14, 38), red.Time);
            Assert.AreEqual(64466, red.TotalXP);

            red = xpRed[17];
            Assert.AreEqual(11379, red.HeroXP);
            Assert.AreEqual(21, red.Level);
            Assert.AreEqual(3661, red.CreepXP);
            Assert.AreEqual(35918, red.MinionXP);
            Assert.AreEqual(29392, red.PassiveXP);
            Assert.AreEqual(2350, red.StructureXP);
            Assert.AreEqual(new TimeSpan(0, 18, 27), red.Time);
            Assert.AreEqual(82700, red.TotalXP);
        }

        [TestMethod]
        public void PlayersScoreResultTest()
        {
            ScoreResult scoreResult = _stormReplay.StormPlayers.ToList()[0].ScoreResult;

            Assert.AreEqual(3, scoreResult.Assists);
            Assert.AreEqual(0, scoreResult.ClutchHealsPerformed);
            Assert.AreEqual(33369, scoreResult.CreepDamage);
            Assert.AreEqual(28369, scoreResult.DamageSoaked);
            Assert.AreEqual(43030, scoreResult.DamageTaken);
            Assert.AreEqual(5, scoreResult.Deaths);
            Assert.AreEqual(0, scoreResult.EscapesPerformed);
            Assert.AreEqual(8968, scoreResult.ExperienceContribution);
            Assert.AreEqual(0, scoreResult.Healing);
            Assert.AreEqual(47875, scoreResult.HeroDamage);
            Assert.AreEqual(5, scoreResult.HighestKillStreak);
            Assert.AreEqual(19, scoreResult.Level);
            Assert.AreEqual(7, scoreResult.MercCampCaptures);
            Assert.AreEqual(70313, scoreResult.MetaExperience);
            Assert.AreEqual(50333, scoreResult.MinionDamage);
            Assert.AreEqual(0, scoreResult.Multikill);
            Assert.AreEqual(2, scoreResult.OutnumberedDeaths);
            Assert.AreEqual(101982, scoreResult.PhysicalDamage);
            Assert.AreEqual(0, scoreResult.ProtectionGivenToAllies);
            Assert.AreEqual(0, scoreResult.SelfHealing);
            Assert.AreEqual(61382, scoreResult.SiegeDamage);
            Assert.AreEqual(3, scoreResult.SoloKills);
            Assert.AreEqual(49618, scoreResult.SpellDamage);
            Assert.AreEqual(5864, scoreResult.StructureDamage);
            Assert.AreEqual(2986, scoreResult.SummonDamage);
            Assert.AreEqual(6, scoreResult.Takedowns);
            Assert.AreEqual(34692, scoreResult.TeamfightDamageTaken);
            Assert.AreEqual(0, scoreResult.TeamfightEscapesPerformed);
            Assert.AreEqual(2531, scoreResult.TeamfightHealingDone);
            Assert.AreEqual(35818, scoreResult.TeamfightHeroDamage);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeCCdEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeRootingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeSilencingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 2, 27), scoreResult.TimeSpentDead);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeStunningEnemyHeroes);
            Assert.AreEqual(0, scoreResult.TownKills);
            Assert.AreEqual(2, scoreResult.VengeancesPerformed);
            Assert.AreEqual(0, scoreResult.WatchTowerCaptures);
            Assert.AreEqual(56, scoreResult.MinionKills);
            Assert.AreEqual(33, scoreResult.RegenGlobes);
        }

        [TestMethod]
        public void PlayersMatchAwardsTest()
        {
            List<MatchAwardType> matchAwards = _stormReplay.StormPlayers.ToList()[0].MatchAwards.ToList();

            Assert.AreEqual(1, _stormReplay.StormPlayers.ToList()[0].MatchAwardsCount);
            Assert.AreEqual(MatchAwardType.MostMercCampsCaptured, matchAwards[0]);

            matchAwards = _stormReplay.StormPlayers.ToList()[9].MatchAwards.ToList();

            Assert.AreEqual(0, matchAwards.Count);
        }

        [TestMethod]
        public void MessagesTest()
        {
            List<StormMessage> messages = _stormReplay.Messages.ToList();

            StormMessage stormMessage = messages.Last();

            Assert.AreEqual(StormMessageEventType.SChatMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Rehgar", stormMessage.MessageSender!.PlayerHero.HeroName);
            Assert.AreEqual(9, stormMessage.PlayerIndex);
            Assert.AreEqual(StormMessageTarget.Allies, stormMessage.ChatMessage!.MessageTarget);
            Assert.IsTrue(stormMessage.ChatMessage!.Message!.StartsWith("https:"));
            Assert.IsTrue(stormMessage.ChatMessage!.Message!.EndsWith("nzs"));
            Assert.AreEqual(new TimeSpan(0, 18, 26), stormMessage.Timestamp);

            stormMessage = messages.First();

            Assert.AreEqual(StormMessageEventType.SLoadingProgressMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Rehgar", stormMessage.MessageSender!.PlayerHero.HeroName);
            Assert.AreEqual(9, stormMessage.PlayerIndex);
            Assert.AreEqual(12, stormMessage.LoadingProgressMessage!.LoadingProgress);
            Assert.AreEqual(new TimeSpan(0, 0, 0), stormMessage.Timestamp);

            stormMessage = messages[97];

            Assert.AreEqual(StormMessageEventType.SPingMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Arthas", stormMessage.MessageSender!.PlayerHero.HeroName);
            Assert.AreEqual(7, stormMessage.PlayerIndex);
            Assert.AreEqual(StormMessageTarget.Allies, stormMessage.PingMessage!.MessageTarget);
            Assert.AreEqual(146.725830078125, stormMessage.PingMessage!.Point!.Value.X);
            Assert.AreEqual(73.9296875, stormMessage.PingMessage!.Point!.Value.Y);
            Assert.AreEqual(new TimeSpan(0, 1, 49), stormMessage.Timestamp);
        }

        [TestMethod]
        public void ChatMessagesTest()
        {
            List<StormMessage> messages = _stormReplay.ChatMessages.ToList();

            Assert.AreEqual(15, messages.Count);
            Assert.IsTrue(messages.All(x => x.ChatMessage != null && !string.IsNullOrEmpty(x.ChatMessage.Message)));
        }
    }
}
