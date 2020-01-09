namespace Heroes.ReplayParser.MessageEvent
{
    /// <summary>
    /// Contains the information for an ability announcement.
    /// </summary>
    public struct AbilityAnnouncement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbilityAnnouncement"/> struct.
        /// </summary>
        /// <param name="abilityIndex"></param>
        /// <param name="abilityLink"></param>
        /// <param name="buttonLink"></param>
        public AbilityAnnouncement(int abilityIndex, int abilityLink, int buttonLink)
        {
            AbilityIndex = abilityIndex;
            AbilityLink = abilityLink;
            ButtonLink = buttonLink;
        }

        /// <summary>
        /// Gets the ability index.
        /// </summary>
        public int AbilityIndex { get; }

        /// <summary>
        /// Gets the ability link number.
        /// </summary>
        public int AbilityLink { get; }

        /// <summary>
        /// Gets the button link number.
        /// </summary>
        public int ButtonLink { get; }
    }
}
