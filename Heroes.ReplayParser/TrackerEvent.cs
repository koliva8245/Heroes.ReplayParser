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
        /// Initializes a new instance of the <see cref="TrackerEvent"/> struct.
        /// </summary>
        /// <param name="trackerEventType">The event type of the tracker.</param>
        /// <param name="timeSpan">The time the event took place.</param>
        /// <param name="versionedDecoder">Data associated with the event.</param>
        public TrackerEvent(TrackerEventType trackerEventType, TimeSpan timeSpan, VersionedDecoder versionedDecoder)
        {
            TrackerEventType = trackerEventType;
            TimeSpan = timeSpan;
            VersionedDecoder = versionedDecoder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerEvent"/> struct.
        /// </summary>
        /// <param name="trackerEventType">The event type of the tracker.</param>
        /// <param name="timeSpan">The time the event took place.</param>
        public TrackerEvent(TrackerEventType trackerEventType, TimeSpan timeSpan)
        {
            TrackerEventType = trackerEventType;
            TimeSpan = timeSpan;
            VersionedDecoder = null;
        }

        /// <summary>
        /// Gets the type of the tracker event.
        /// </summary>
        public TrackerEventType TrackerEventType { get; }

        /// <summary>
        /// Gets the time that the event took place.
        /// </summary>
        public TimeSpan TimeSpan { get; }

        /// <summary>
        /// Gets the version decoder to obtain the data.
        /// </summary>
        public VersionedDecoder? VersionedDecoder { get; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return $"{TrackerEventType.ToString()}: {VersionedDecoder?.ToString()}";
        }
    }
}
