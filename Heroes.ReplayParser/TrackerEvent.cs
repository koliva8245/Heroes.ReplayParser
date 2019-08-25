using Heroes.ReplayParser.Decoders;
using System;

namespace Heroes.ReplayParser
{
    public class TrackerEvent
    {
        public TrackerEventType TrackerEventType { get; set; }

        public TimeSpan TimeSpan { get; set; }

        internal VersionedDecoder? VersionedDecoder { get; set; }

        public override string? ToString()
        {
            return $"{TrackerEventType.ToString()}: {VersionedDecoder?.ToString()}";
        }
    }
}
