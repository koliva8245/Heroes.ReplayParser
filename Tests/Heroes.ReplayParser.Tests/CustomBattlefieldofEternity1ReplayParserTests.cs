﻿using Heroes.ReplayParser.MessageEvent;
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
    public class CustomBattlefieldofEternity1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly StormReplay _stormReplay;

        public CustomBattlefieldofEternity1ReplayParserTests()
        {
            _stormReplay = StormReplayParser.Parse(Path.Combine(_replaysFolder, "CustomBattlefieldofEternity1_65006.StormR"), out StormReplayParseResult _);
        }

        [TestMethod]
        public void StormReplayHeaderTest()
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
        public void StormReplayDetailsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("AZTDoubt", player0.Name);
            Assert.AreEqual(1, player0.ToonHandle.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Blue, player0.Team);
            Assert.IsFalse(player0.IsWinner);
            Assert.AreEqual("Greymane", player0.PlayerHero.HeroName);

            StormPlayer player = players[9];

            Assert.AreEqual("FiZiX", player.Name);
            Assert.AreEqual(1, player.ToonHandle.Region);
            Assert.AreEqual(1, player.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Red, player.Team);
            Assert.IsTrue(player.IsWinner);
            Assert.AreEqual("Li-Ming", player.PlayerHero.HeroName);

            Assert.AreEqual("Battlefield of Eternity", _stormReplay.MapInfo.MapName);
            Assert.AreEqual(636619794857150779, _stormReplay.Timestamp.Ticks);

            List<StormPlayer> playersWithObs = _stormReplay.StormPlayersWithObservers.ToList();
            StormPlayer player8 = playersWithObs[8];

            Assert.AreEqual(StormTeam.Observer, player8.Team);

            Assert.IsTrue(_stormReplay.HasObservers);
            Assert.IsFalse(_stormReplay.HasAI);
            Assert.AreEqual(1, _stormReplay.PlayersObserversCount);
            Assert.AreEqual(string.Empty, _stormReplay.StormObservers.ToList()[0].PlayerHero.HeroName);
        }

        [TestMethod]
        public void StormReplayInitDataTest()
        {
            Assert.AreEqual(36047320, _stormReplay.RandomValue);
            Assert.AreEqual(GameMode.Custom, _stormReplay.GameMode);

            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("GreymaneDoctorVar3", player0.PlayerLoadout.SkinAndSkinTint);
            Assert.AreEqual("MountCloudWhimsy", player0.PlayerLoadout.MountAndMountTint);
            Assert.IsFalse(player0.IsSilenced);
            Assert.IsFalse(player0.IsVoiceSilenced);
            Assert.IsFalse(player0.IsBlizzardStaff);
            Assert.IsFalse(player0.HasActiveBoost);
            Assert.AreEqual("BannerDFEsportsWarChestRareDignitas", player0.PlayerLoadout.Banner);
            Assert.AreEqual("SprayStaticHGC2017EUDignitas", player0.PlayerLoadout.Spray);
            Assert.AreEqual("JainaA", player0.PlayerLoadout.AnnouncerPack);
            Assert.AreEqual("GreymaneBase_VoiceLine01", player0.PlayerLoadout.VoiceLine);
            Assert.AreEqual(15, player0.HeroMasteryTiersCount);
            Assert.AreEqual("Barb", player0.HeroMasteryTiers.ToList()[2].HeroAttributeId);
            Assert.AreEqual(1, player0.HeroMasteryTiers.ToList()[2].TierLevel);
            Assert.AreEqual(PlayerType.Human, player0.PlayerType);

            List<StormPlayer> playersWithObs = _stormReplay.StormPlayersWithObservers.ToList();
            StormPlayer player8 = playersWithObs[8];

            Assert.AreEqual(PlayerType.Observer, player8.PlayerType);
            Assert.AreEqual(PlayerDifficulty.Unknown, player8.PlayerDifficulty);
        }

        [TestMethod]
        public void StormReplayAttributeEventsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player = players[9];

            Assert.AreEqual("5v5", _stormReplay.TeamSize);
            Assert.AreEqual(PlayerDifficulty.Elite, player.PlayerDifficulty);
            Assert.AreEqual(GameSpeed.Faster, _stormReplay.GameSpeed);
            Assert.AreEqual(GameMode.Custom, _stormReplay.GameMode);
            Assert.AreEqual("Wiza", player.PlayerHero.HeroAttributeId);
            Assert.AreEqual("WizI", player.PlayerLoadout.SkinAndSkinTintAttributeId);
            Assert.AreEqual("CLO0", player.PlayerLoadout.MountAndMountTintAttributeId);
            Assert.AreEqual("BN6d", player.PlayerLoadout.BannerAttributeId);
            Assert.AreEqual("SY81", player.PlayerLoadout.SprayAttributeId);
            Assert.AreEqual("WZ04", player.PlayerLoadout.VoiceLineAttributeId);
            Assert.AreEqual("AFIR", player.PlayerLoadout.AnnouncerPackAttributeId);
            Assert.AreEqual(20, player.PlayerHero.HeroLevel);

            List<string?> ban0List = _stormReplay.GetTeamBans(StormTeam.Blue).ToList();
            List<string?> ban1List = _stormReplay.GetTeamBans(StormTeam.Red).ToList();

            Assert.AreEqual("Diab", ban0List[1]);
            Assert.AreEqual("Tra0", ban1List[1]);
        }

        [TestMethod]
        public void DraftOrderTest()
        {
            List<DraftPick> draft = _stormReplay.GetDraftOrder().ToList();

            Assert.AreEqual(14, draft.Count);

            Assert.AreEqual("Maiev", draft[0].HeroSelected);
            Assert.AreEqual(DraftPickType.Banned, draft[0].PickType);
            Assert.AreEqual(2, draft[0].SelectedPlayerSlotId);

            Assert.AreEqual("Kaelthas", draft[13].HeroSelected);
            Assert.AreEqual(DraftPickType.Picked, draft[13].PickType);
            Assert.AreEqual(4, draft[13].SelectedPlayerSlotId);
        }

        [TestMethod]
        public void TeamLevelsTest()
        {
            List<TeamLevel> levelsBlue = _stormReplay.GetTeamLevels(StormTeam.Blue).ToList();
            List<TeamLevel> levelsRed = _stormReplay.GetTeamLevels(StormTeam.Red).ToList();

            Assert.AreEqual(18, levelsBlue.Count);
            Assert.AreEqual(20, levelsRed.Count);

            Assert.AreEqual(1, levelsBlue[0].Level);
            Assert.AreEqual(new TimeSpan(32500000), levelsBlue[0].Time);

            Assert.AreEqual(8, levelsBlue[7].Level);
            Assert.AreEqual(new TimeSpan(3701875000), levelsBlue[7].Time);

            Assert.AreEqual(18, levelsBlue[17].Level);
            Assert.AreEqual(new TimeSpan(11347500000), levelsBlue[17].Time);

            Assert.AreEqual(1, levelsRed[0].Level);
            Assert.AreEqual(new TimeSpan(32500000), levelsRed[0].Time);

            Assert.AreEqual(10, levelsRed[9].Level);
            Assert.AreEqual(new TimeSpan(5124375000), levelsRed[9].Time);

            Assert.AreEqual(20, levelsRed[19].Level);
            Assert.AreEqual(new TimeSpan(11850625000), levelsRed[19].Time);
        }

        [TestMethod]
        public void TeamsFinalLevelTest()
        {
            Assert.AreEqual(18, _stormReplay.GetTeamFinalLevel(StormTeam.Blue));
            Assert.AreEqual(20, _stormReplay.GetTeamFinalLevel(StormTeam.Red));
            Assert.AreEqual(0, _stormReplay.GetTeamFinalLevel(StormTeam.Observer));
        }

        [TestMethod]
        public void TeamXpBreakdownTest()
        {
            List<TeamXPBreakdown> xpBlue = _stormReplay.GetTeamXPBreakdown(StormTeam.Blue).ToList();
            List<TeamXPBreakdown> xpRed = _stormReplay.GetTeamXPBreakdown(StormTeam.Red).ToList();
            List<TeamXPBreakdown> xpOther = _stormReplay.GetTeamXPBreakdown(StormTeam.Observer).ToList();

            Assert.AreEqual(20, xpBlue.Count);
            Assert.AreEqual(20, xpRed.Count);
            Assert.AreEqual(0, xpOther.Count);

            TeamXPBreakdown blue = xpBlue[3];

            Assert.AreEqual(1272, blue.HeroXP);
            Assert.AreEqual(5, blue.Level);
            Assert.AreEqual(360, blue.CreepXP);
            Assert.AreEqual(4868, blue.MinionXP);
            Assert.AreEqual(4100, blue.PassiveXP);
            Assert.AreEqual(0, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(2781250000), blue.Time);
            Assert.AreEqual(10600, blue.TotalXP);

            blue = xpBlue[19];
            Assert.AreEqual(6037, blue.HeroXP);
            Assert.AreEqual(18, blue.Level);
            Assert.AreEqual(4668, blue.CreepXP);
            Assert.AreEqual(21883, blue.MinionXP);
            Assert.AreEqual(22520, blue.PassiveXP);
            Assert.AreEqual(7250, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(12065000000), blue.Time);
            Assert.AreEqual(62358, blue.TotalXP);

            TeamXPBreakdown red = xpRed[3];

            Assert.AreEqual(0, red.HeroXP);
            Assert.AreEqual(5, red.Level);
            Assert.AreEqual(225, red.CreepXP);
            Assert.AreEqual(5082, red.MinionXP);
            Assert.AreEqual(4100, red.PassiveXP);
            Assert.AreEqual(0, red.StructureXP);
            Assert.AreEqual(new TimeSpan(2781250000), red.Time);
            Assert.AreEqual(9407, red.TotalXP);

            red = xpRed[19];
            Assert.AreEqual(12729, red.HeroXP);
            Assert.AreEqual(20, red.Level);
            Assert.AreEqual(6083, red.CreepXP);
            Assert.AreEqual(23551, red.MinionXP);
            Assert.AreEqual(22520, red.PassiveXP);
            Assert.AreEqual(8850, red.StructureXP);
            Assert.AreEqual(new TimeSpan(12065000000), red.Time);
            Assert.AreEqual(73733, red.TotalXP);
        }

        [TestMethod]
        public void PlayersScoreResultTest()
        {
            StormPlayer player = _stormReplay.StormPlayers.ToList()[8];

            Assert.AreEqual("Malfurion", player.PlayerHero.HeroName);

            ScoreResult scoreResult = player.ScoreResult;

            Assert.AreEqual(8, scoreResult.Assists);
            Assert.AreEqual(3, scoreResult.ClutchHealsPerformed);
            Assert.AreEqual(8266, scoreResult.CreepDamage);
            Assert.AreEqual(18772, scoreResult.DamageSoaked);
            Assert.AreEqual(21863, scoreResult.DamageTaken);
            Assert.AreEqual(1, scoreResult.Deaths);
            Assert.AreEqual(0, scoreResult.EscapesPerformed);
            Assert.AreEqual(7123, scoreResult.ExperienceContribution);
            Assert.AreEqual(65166, scoreResult.Healing);
            Assert.AreEqual(13986, scoreResult.HeroDamage);
            Assert.AreEqual(8, scoreResult.HighestKillStreak);
            Assert.AreEqual(18, scoreResult.Level);
            Assert.AreEqual(0, scoreResult.MercCampCaptures);
            Assert.AreEqual(62359, scoreResult.MetaExperience);
            Assert.AreEqual(12804, scoreResult.MinionDamage);
            Assert.AreEqual(0, scoreResult.Multikill);
            Assert.AreEqual(0, scoreResult.OutnumberedDeaths);
            Assert.AreEqual(null, scoreResult.PhysicalDamage);
            Assert.AreEqual(0, scoreResult.ProtectionGivenToAllies);
            Assert.AreEqual(0, scoreResult.SelfHealing);
            Assert.AreEqual(22012, scoreResult.SiegeDamage);
            Assert.AreEqual(0, scoreResult.SoloKills);
            Assert.AreEqual(null, scoreResult.SpellDamage);
            Assert.AreEqual(9208, scoreResult.StructureDamage);
            Assert.AreEqual(0, scoreResult.SummonDamage);
            Assert.AreEqual(8, scoreResult.Takedowns);
            Assert.AreEqual(12196, scoreResult.TeamfightDamageTaken);
            Assert.AreEqual(0, scoreResult.TeamfightEscapesPerformed);
            Assert.AreEqual(16378, scoreResult.TeamfightHealingDone);
            Assert.AreEqual(5228, scoreResult.TeamfightHeroDamage);
            Assert.AreEqual(new TimeSpan(0, 0, 28), scoreResult.TimeCCdEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 25), scoreResult.TimeRootingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeSilencingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 56), scoreResult.TimeSpentDead);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeStunningEnemyHeroes);
            Assert.AreEqual(0, scoreResult.TownKills);
            Assert.AreEqual(0, scoreResult.VengeancesPerformed);
            Assert.AreEqual(0, scoreResult.WatchTowerCaptures);
        }

        [TestMethod]
        public void PlayersMatchAwardsTest()
        {
            List<MatchAwardType> matchAwards = _stormReplay.StormPlayers.ToList()[8].MatchAwards.ToList();

            Assert.AreEqual(0, matchAwards.Count);
        }

        [TestMethod]
        public void MessagesTest()
        {
            List<StormMessage> messages = _stormReplay.Messages.ToList();

            StormMessage stormMessage = messages[144];

            Assert.AreEqual(StormMessageEventType.SPlayerAnnounceMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Li Li", stormMessage.MessageSender!.PlayerHero.HeroName);
            Assert.AreEqual(0, stormMessage.PlayerAnnounceMessage!.AbilityAnnouncement!.Value.AbilityIndex);
            Assert.AreEqual(423, stormMessage.PlayerAnnounceMessage!.AbilityAnnouncement!.Value.AbilityLink);
            Assert.AreEqual(954, stormMessage.PlayerAnnounceMessage!.AbilityAnnouncement!.Value.ButtonLink);
            Assert.AreEqual(new TimeSpan(7110000000), stormMessage.Timestamp);

            stormMessage = messages.Last();

            Assert.AreEqual(StormMessageEventType.SChatMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Li-Ming", stormMessage.MessageSender!.PlayerHero.HeroName);
            Assert.AreEqual(new TimeSpan(12031875000), stormMessage.Timestamp);
            Assert.AreEqual(StormMessageTarget.All, stormMessage.ChatMessage!.MessageTarget);
        }

        [TestMethod]
        public void ChatMessagesTest()
        {
            List<StormMessage> messages = _stormReplay.ChatMessages.ToList();

            Assert.AreEqual(4, messages.Count);
            Assert.IsTrue(messages.All(x => x.ChatMessage != null && !string.IsNullOrEmpty(x.ChatMessage.Message)));
        }
    }
}
