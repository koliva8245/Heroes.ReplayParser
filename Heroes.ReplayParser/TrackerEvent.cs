using Heroes.ReplayParser.Decoders;
using System;

namespace Heroes.ReplayParser
{
    /// <summary>
    /// Contains the properties for a tracker event.
    /// </summary>
    public struct TrackerEvent
    {
        /// <summary>
        /// Gets or sets the type of the tracker event.
        /// </summary>
        public TrackerEventType TrackerEventType;

        /// <summary>
        /// Gets or sets the time that the event took place.
        /// </summary>
        public TimeSpan TimeSpan;

        internal VersionedDecoder? VersionedDecoder;

        /// <inheritdoc/>
        public override string? ToString()
        {
            return $"{TrackerEventType.ToString()}: {VersionedDecoder?.ToString()}";
        }
    }
}
