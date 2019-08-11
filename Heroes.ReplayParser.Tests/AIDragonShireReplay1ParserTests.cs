﻿using Heroes.ReplayParser.Player;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        public AIDragonShireReplay1ParserTests()
        {
            _stormReplay = StormReplayParser.Parse(Path.Combine(_replaysFolder, "AIDragonShire1_75589.StormReplay"));
        }

        [TestMethod]
        public void StormReplayHeaderTests()
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
        public void StormReplayDetailsTests()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("lavakill", player0.Name);
            Assert.AreEqual(1, player0.ToonHandle.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(0, player0.Team);
            Assert.IsTrue(player0.IsWinner);
            Assert.AreEqual("Qhira", player0.HeroName);

            StormPlayer player1 = players[9];

            Assert.AreEqual("Player 10", player1.Name);
            Assert.AreEqual(0, player1.ToonHandle.Region);
            Assert.AreEqual(0, player1.ToonHandle.Realm);
            Assert.AreEqual(1, player1.Team);
            Assert.IsFalse(player1.IsWinner);
            Assert.AreEqual("Valeera", player1.HeroName);

            Assert.AreEqual("Dragon Shire", _stormReplay.MapName);
            Assert.AreEqual(637010888527768698, _stormReplay.Timestamp.Ticks);
        }
    }
}
