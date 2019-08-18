using System.Collections.Generic;

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
        /// Gets or sets the player's hero information.
        /// </summary>
        public PlayerHero PlayerHero { get; set; } = new PlayerHero();

        /// <summary>
        /// Gets or sets the player's loadout information.
        /// </summary>
        public PlayerLoadout PlayerLoadout { get; set; } = new PlayerLoadout();

        /// <summary>
        /// Gets or sets the player's hero's mastery tier levels.
        /// </summary>
        public IList<HeroMasteryTier> HeroMasteryTiers { get; set; } = new List<HeroMasteryTier>();

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
        /// Gets or sets if the player has been given the silenced penalty.
        /// </summary>
        public bool IsSilenced { get; set; } = false;

        /// <summary>
        /// Gets or sets if the player has been given the voice silence penalty.
        /// </summary>
        public bool IsVoiceSilenced { get; set; } = false;

        /// <summary>
        /// Gets or sets if the player is Blizzard staff.
        /// </summary>
        public bool IsBlizzardStaff { get; set; } = false;

        /// <summary>
        /// Gets or sets if the player has an active boost.
        /// </summary>
        public bool HasActiveBoost { get; set; } = false;

        internal int WorkingSetSlotId { get; set; }

        public override string? ToString()
        {
            return $"{Name}-{PlayerType.ToString()}-{ToonHandle}";
        }
    }
}
