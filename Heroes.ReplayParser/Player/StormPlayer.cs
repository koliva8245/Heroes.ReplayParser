namespace Heroes.ReplayParser.Player
{
    public class StormPlayer
    {
        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's toon handle.
        /// </summary>
        public ToonHandle ToonHandle { get; private set; } = new ToonHandle();

        /// <summary>
        /// Gets or sets the player's control type.
        /// </summary>
        public PlayerType PlayerType { get; set; }

        /// <summary>
        /// Gets or sets the player's team id.
        /// </summary>
        public int Team { get; set; }

        /// <summary>
        /// Gets or sets the player's handicap.
        /// </summary>
        public int Handicap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player won the game.
        /// </summary>
        public bool IsWinner { get; set; }

        /// <summary>
        /// Gets or sets the player's hero name.
        /// </summary>
        public string HeroName { get; set; } = string.Empty;

        internal int WorkingSetSlotId { get; set; }
    }
}
