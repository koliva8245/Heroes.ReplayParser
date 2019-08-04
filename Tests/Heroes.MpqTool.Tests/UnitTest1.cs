using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Heroes.MpqTool.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            MpqArchive mpqArchive = new MpqArchive("HanamuraTemple1.StormReplay");
        }
    }
}
