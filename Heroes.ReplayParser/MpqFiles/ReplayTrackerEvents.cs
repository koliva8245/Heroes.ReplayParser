using Heroes.MpqToolV2;
using Heroes.ReplayParser.Decoders;
using Heroes.ReplayParser.Replay;
using System.Collections.Generic;

namespace Heroes.ReplayParser.MpqFiles
{
    internal class ReplayTrackerEvents : IMpqFiles
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
