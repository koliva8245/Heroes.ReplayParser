using Heroes.ReplayParser.Replay;
using System;

namespace Heroes.ReplayParser
{
    public class StormReplayResult
    {
        public StormReplayParseStatus Status { get; }

        public Exception Exception { get; }

        public StormReplay Replay { get; }

        public StormReplayResult(StormReplay stormReplay, StormReplayParseStatus stormReplayParseStatus)
        {
            Replay = stormReplay;
            Status = stormReplayParseStatus;
        }

        public StormReplayResult(StormReplay stormReplay, StormReplayParseStatus stormReplayParseStatus, Exception exception)
        {
            Replay = stormReplay;
            Status = stormReplayParseStatus;
            Exception = exception;
        }
    }
}
