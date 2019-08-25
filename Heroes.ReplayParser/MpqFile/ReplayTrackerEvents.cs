using System;
using System.Collections.Generic;
using System.Text;
using Heroes.MpqTool;
using Heroes.ReplayParser.Decoders;
using Heroes.ReplayParser.Replay;

namespace Heroes.ReplayParser.MpqFile
{
    internal class ReplayTrackerEvents : IMpqFile
    {
        public string FileName { get; } = "replay.tracker.events";

        public void Parse(StormReplay stormReplay, MpqBuffer mpqBuffer)
        {
            List<TrackerEvent> trackerEvents = new List<TrackerEvent>();

            int gameLoop = 0;

            while (!mpqBuffer.IsEndOfBuffer)
            {
                TrackerEvent trackerEvent = new TrackerEvent();

                VersionedDecoder versionDecoder = new VersionedDecoder(mpqBuffer);

                trackerEvent.TrackerEventType = (TrackerEventType)new VersionedDecoder(mpqBuffer).GetValueAsUInt32();
                trackerEvent.VersionedDecoder = new VersionedDecoder(mpqBuffer);

                trackerEvents.Add(trackerEvent);
            }
        }
    }
}
