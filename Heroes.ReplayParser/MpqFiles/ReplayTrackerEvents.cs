using Heroes.MpqToolV2;
using Heroes.ReplayParser.Decoders;
using Heroes.ReplayParser.Replay;
using System;
using System.Collections.Generic;

namespace Heroes.ReplayParser.MpqFiles
{
    internal class ReplayTrackerEvents : IMpqFiles
    {
        public string FileName { get; } = "replay.tracker.events";

        public void Parse(StormReplay stormReplay, ReadOnlySpan<byte> source)
        {
            List<TrackerEvent> trackerEvents = new List<TrackerEvent>();

            int gameLoop = 0;

            while (!source.IsEmpty)
            {
                TrackerEvent trackerEvent = new TrackerEvent();

                VersionedDecoder versionDecoder = new VersionedDecoder(source);

                trackerEvent.TrackerEventType = (TrackerEventType)new VersionedDecoder(source).GetValueAsUInt32();
                trackerEvent.VersionedDecoder = new VersionedDecoder(source);

                trackerEvents.Add(trackerEvent);
            }
        }
    }
}
