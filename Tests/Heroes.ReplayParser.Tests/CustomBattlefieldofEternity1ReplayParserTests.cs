using Heroes.ReplayParser.Player;
using Heroes.ReplayParser.Replay;
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
            Assert.AreEqual(15, player0.HeroMasteryTiers.Count);
            Assert.AreEqual("Barb", player0.HeroMasteryTiers[2].HeroAttributeId);
            Assert.AreEqual(1, player0.HeroMasteryTiers[2].TierLevel);
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
        public void GetDraftOrderTest()
        {
            var draft = _stormReplay.GetDraftOrder().ToList();

            Assert.AreEqual(14, draft.Count);

            Assert.AreEqual("Maiev", draft[0].HeroSelected);
            Assert.AreEqual(DraftPickType.Banned, draft[0].PickType);
            Assert.AreEqual(2, draft[0].SelectedPlayerSlotId);

            Assert.AreEqual("Kaelthas", draft[13].HeroSelected);
            Assert.AreEqual(DraftPickType.Picked, draft[13].PickType);
            Assert.AreEqual(4, draft[13].SelectedPlayerSlotId);
        }
    }
}
