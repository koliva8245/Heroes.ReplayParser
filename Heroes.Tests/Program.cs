using Heroes.MpqToolV2;
using Heroes.ReplayParser;
using Heroes.ReplayParser.MpqFiles;
using Heroes.ReplayParser.Replay;
using System.IO;
using System.IO.Compression;
using System.Linq;


namespace Heroes.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //MpqArchive mpqArchive = new MpqArchive("HanamuraTemple1.StormReplay");
            //mpqArchive.AddListfileFilenames();

            // StormReplay stormReplay = StormReplayParser.Parse(@"F:\Battlefield of Eternity1.StormReplay", out StormReplayParseResult _);
            // DirectoryInfo info = new DirectoryInfo(@"C:\Users\koliva\Documents\Heroes of the Storm\Accounts\77558904\1-Hero-1-1527252\Replays\Multiplayer");
            //DirectoryInfo info = new DirectoryInfo(@"C:\Users\koliva\Documents\Heroes of the Storm\bl-collection\test");
            //System.Collections.Generic.IEnumerable<FileInfo> files = info.GetFiles().OrderByDescending(p => p.LastWriteTime).Skip(0);
            //foreach (FileInfo file in files)
            //{
            //    System.Console.Write($"{file.Name}");

            //    StormReplayResult stormReplayResult = StormReplayParser.Parse(file.FullName);
            //    int count = stormReplayResult.Replay.PlayersWithObserversCount;
            //    System.Console.WriteLine($" - {stormReplayResult.Replay.ReplayVersion.ToString()}");

            //    if (stormReplayResult.Replay.GameMode != GameMode.Cooperative && stormReplayResult.Status != StormReplayParseStatus.Success)
            //    {
            //        string x = "";
            //    }
            // }
            //      var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\Accounts\77558904\1-Hero-1-1527252\Replays\Multiplayer\2020-01-24 20.53.42 Towers of Doom.StormReplay"); // 77981
            // var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\Accounts\77558904\09.06.19__Game_1_2019_Division_S_America_-_Phase_1.StormReplay"); // 74238
            //  var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\Accounts\77558904\1-Hero-1-1527252\Replays\Multiplayer\Cursed Hollow (339).StormReplay"); // 47219
            //  var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\Accounts\77558904\1-Hero-1-1527252\Replays\Multiplayer\Braxis Holdout (65).StormReplay"); // 47133
            //var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\Accounts\77558904\1-Hero-1-1527252\Replays\Multiplayer\Tomb of the Spider Queen (272) - Copy.StormReplay"); // 45635
            // var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\Accounts\77558904\1-Hero-1-1527252\Replays\Multiplayer\test\Sky Temple (319).StormReplay");
            // var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\Accounts\77558904\2020-01-24_11.04.35_Lost_Cavern - Copy.StormReplay");
            //var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\MissingWorkSetSlotId\Replays (2)\replay-24321970.StormReplay");
            //var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\MissingWorkSetSlotId\corruptReplay\replay-24368334-bl.StormReplay");
            //  var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\MissingWorkSetSlotId\Replays (2)\replay-24321970 - Copy.StormReplay");
            // var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\76003\2019-09-02 18.35.00 Cursed Hollow.StormReplay");
            //
            //  var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\bl-collection\test\Infernal Shrines (293) - Copy.StormReplay"); // 69350
            //   var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\bl-collection\Lost Cavern (42).StormReplay"); // 
            // var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\bl-collection\Dragon Shire (281).StormReplay"); // 69350
            // var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\Accounts\77558904\98-Hero-1-263640\Replays\Multiplayer\2019-11-27 19.22.55 Braxis Holdout.StormReplay");



            //  var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\playerSilenced\Towers of Doom (280).StormReplay");
            // var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\playerSilenced\Hanamura Temple (23).StormReplay");

            var stormReplay = StormReplayParser.Parse(@"C:\Users\koliva\Documents\Heroes of the Storm\ai\2020-02-07 22.30.28 Cursed Hollow.StormReplay");






            //BitReader.EndianType = EndianType.LittleEndian;
            //BitReader.EndianType = EndianType.LittleEndian;
            //StormReplay stormReplay2 = StormReplayParser.Parse(@"F:\Battlefield of Eternity1.StormReplay");

            //int a = stormReplay.ReplayVersion.Major;
            System.Console.WriteLine("Done");



            //MpqFile.Open(@"F:\Battlefield of Eternity1.StormReplay");

           // FileStream fileStream = new FileStream(@"F:\Battlefield of Eternity1.StormReplay", FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, false);

            //MpqArchive mpqArchive = new MpqArchive(fileStream);
           // mpqArchive.AddListfileFileNames();

           // var a = mpqArchive.OpenFile("replay.details");
            //a.ReadInt32();
        }
    }
}
