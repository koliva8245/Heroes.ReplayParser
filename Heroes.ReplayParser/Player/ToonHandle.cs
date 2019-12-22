﻿namespace Heroes.ReplayParser.Player
{
    /// <summary>
    /// Contains the properties for a player's toon handle.
    /// </summary>
    public class ToonHandle
    {
        /// <summary>
        /// Gets or sets the region value.
        /// </summary>
        public int Region { get; set; }

        /// <summary>
        /// Gets or sets the program id. This id is the same for all player's in this replay.
        /// </summary>
        public long ProgramId { get; set; }

        /// <summary>
        /// Gets or sets the realm value.
        /// </summary>
        public int Realm { get; set; }

        /// <summary>
        /// Gets or sets the id unique to the player's account in this region.
        /// </summary>
        public long Id { get; set; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return $"{Region}-Hero-{Realm}-{Id}";
        }
    }
}
