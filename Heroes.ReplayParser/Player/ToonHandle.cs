namespace Heroes.ReplayParser.Player
{
    public class ToonHandle
    {
        /// <summary>
        /// Gets or sets the region value.
        /// </summary>
        public int Region { get; set; }

        /// <summary>
        /// Gets or sets the program id. This id is the same for all player's in this replay.
        /// </summary>
        public int ProgramId { get; set; }

        /// <summary>
        /// Gets or sets the realm value.
        /// </summary>
        public int Realm { get; set; }

        /// <summary>
        /// Gets or sets the id unique to the player's account in this region.
        /// </summary>
        public int Id { get; set; }
    }
}
