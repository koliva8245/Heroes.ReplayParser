namespace Heroes.ReplayParser.MessageEvent
{
    /// <summary>
    /// Contains the information for a player announce message.
    /// </summary>
    public class PlayerAnnounceMessage
    {
        /// <summary>
        /// Gets or sets the type of announcment.
        /// </summary>
        public AnnouncementType AnnouncementType { get; internal set; }

        /// <summary>
        /// Get or sets the ability announcement.
        /// </summary>
        public AbilityAnnouncement? AbilityAnnouncement { get; internal set; } = null;

        /// <summary>
        /// Gets or sets the behavior announcment.
        /// </summary>
        public BehaviorAnnouncment? BehaviorAnnouncement { get; internal set; } = null;

        /// <summary>
        /// Gets or sets the vital announcement.
        /// </summary>
        public VitalAnnouncement? VitalAnnouncement { get; internal set; } = null;
    }
}
