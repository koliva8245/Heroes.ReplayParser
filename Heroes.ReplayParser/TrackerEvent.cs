using Heroes.ReplayParser.Decoders;
using System;

namespace Heroes.ReplayParser
{
    /// <summary>
    /// Contains the properties for a tracker event.
    /// </summary>
    public class TrackerEvent
    {
        /// <summary>
        /// Gets or sets the type of the tracker event.
        /// </summary>
        public TrackerEventType TrackerEventType { get; set; }

        /// <summary>
        /// Gets or sets the time that the event took place.
        /// </summary>
        public TimeSpan TimeSpan { get; set; }

        internal VersionedDecoder? VersionedDecoder { get; set; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return $"{TrackerEventType.ToString()}: {VersionedDecoder?.ToString()}";
        }
    }
}
