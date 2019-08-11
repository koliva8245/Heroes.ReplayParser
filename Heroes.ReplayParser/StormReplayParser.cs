using Heroes.MpqTool;
using Heroes.ReplayParser.MpqFile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Heroes.ReplayParser
{
    public class StormReplayParser
    {
        private readonly string _fileName;
        private readonly bool _allowPTRRegion;
        private readonly bool _parseBattleLobby;

        private readonly MpqArchive _stormMpqArchive;
        private readonly StormReplay _stormReplay = new StormReplay();

        private StormReplayParser(string fileName, bool allowPTRRegion, bool parseBattleLobby)
        {
            _fileName = fileName;
            _allowPTRRegion = allowPTRRegion;
            _parseBattleLobby = parseBattleLobby;

            _stormMpqArchive = new MpqArchive(_fileName);
        }

        /// <summary>
        /// Parses a .StormReplay file.
        /// </summary>
        /// <param name="fileName">The file name which may contain the path.</param>
        /// <param name="allowPTRRegion"></param>
        /// <param name="parseBattleLobby">If enabled, the battle lobby file will be pared which gives more available data.</param>
        public static StormReplay Parse(string fileName, bool allowPTRRegion = false, bool parseBattleLobby = false)
        {
            StormReplayParser stormReplayParser = new StormReplayParser(fileName, allowPTRRegion, parseBattleLobby);
            stormReplayParser.Parse();

            return stormReplayParser._stormReplay;
        }

        private void Parse()
        {
            _stormMpqArchive.AddListfileFilenames();

            StormReplayHeader.Parse(_stormReplay, _stormMpqArchive.MpqBuffer);

            //if (!ignoreErrors && replay.ReplayBuild < 32455)
            //    return new Tuple<ReplayParseResult, Replay>(ReplayParseResult.PreAlphaWipe, new Replay { ReplayBuild = replay.ReplayBuild });

            ReplayDetails replayDetails = new ReplayDetails();
            replayDetails.Parse(_stormReplay, _stormMpqArchive.OpenFile(replayDetails.FileName));
        }
    }
}
