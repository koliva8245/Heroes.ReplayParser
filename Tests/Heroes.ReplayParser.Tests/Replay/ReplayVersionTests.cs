using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Heroes.ReplayParser.Replay.Tests
{
    [TestClass]
    public class ReplayVersionTests
    {
        [TestMethod]
        public void EqualsTest()
        {
            ReplayVersion replayVersion1 = new ReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33444,
                BaseBuild = 33444,
            };

            ReplayVersion replayVersion2 = new ReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33444,
                BaseBuild = 33444,
            };

            Assert.AreEqual(replayVersion1, replayVersion2);
            Assert.AreEqual(replayVersion2, replayVersion1);
        }

        [TestMethod]
        public void NotEqualsTest()
        {
            ReplayVersion replayVersion1 = new ReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33443,
                BaseBuild = 33444,
            };

            ReplayVersion replayVersionMajor = new ReplayVersion()
            {
                Major = 2,
                Minor = 3,
                Revision = 23,
                Build = 33443,
                BaseBuild = 33444,
            };

            ReplayVersion replayVersionMinor = new ReplayVersion()
            {
                Major = 1,
                Minor = 4,
                Revision = 23,
                Build = 33443,
                BaseBuild = 33444,
            };

            ReplayVersion replayVersionRevision = new ReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 24,
                Build = 33443,
                BaseBuild = 33444,
            };

            ReplayVersion replayVersionBuild = new ReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33444,
                BaseBuild = 33444,
            };

            ReplayVersion replayVersionBaseBuild = new ReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33443,
                BaseBuild = 33442,
            };

            Assert.AreNotEqual(replayVersion1, replayVersionMajor);
            Assert.AreNotEqual(replayVersion1, replayVersionMinor);
            Assert.AreNotEqual(replayVersion1, replayVersionRevision);
            Assert.AreNotEqual(replayVersion1, replayVersionBuild);
            Assert.AreNotEqual(replayVersion1, replayVersionBaseBuild);
        }

        [TestMethod]
        public void NotEqualsDiffObjects()
        {
            ReplayVersion replayVersion1 = new ReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33444,
                BaseBuild = 33444,
            };

            Assert.AreNotEqual(replayVersion1, new MapInfo());
        }
    }
}