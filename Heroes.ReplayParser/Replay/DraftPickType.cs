namespace Heroes.ReplayParser.Replay
{
    /// <summary>
    /// Specifices the type of draft pick.
    /// </summary>
    public enum DraftPickType
    {
        /// <summary>
        /// Indicates a banned type.
        /// </summary>
        Banned = 0,

        /// <summary>
        /// Indicates a picked type.
        /// </summary>
        Picked = 1,

        /// <summary>
        /// Indicates a swapped type.
        /// </summary>
        Swapped = 2,
    }
}
