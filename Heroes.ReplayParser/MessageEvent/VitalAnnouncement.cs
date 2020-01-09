namespace Heroes.ReplayParser.MessageEvent
{
    /// <summary>
    /// Contains the information for a vital announcment.
    /// </summary>
    public struct VitalAnnouncement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VitalAnnouncement"/> struct.
        /// </summary>
        /// <param name="vitalType">The type of vital.</param>
        public VitalAnnouncement(VitalType vitalType)
        {
            VitalType = vitalType;
        }

        /// <summary>
        /// Gets the type of vital.
        /// </summary>
        public VitalType VitalType { get; }
    }
}
