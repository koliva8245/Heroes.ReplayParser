namespace Heroes.ReplayParser.Player
{
    public class PlayerHero
    {
        /// <summary>
        /// Gets or sets the hero id.
        /// </summary>
        public string HeroId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hero name.
        /// </summary>
        public string HeroName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hero attribute id.
        /// </summary>
        public string HeroAttributeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hero's level.
        /// </summary>
        public int HeroLevel { get; set; } = 0;

        public override string? ToString()
        {
            return $"{HeroId}-{HeroName}";
        }
    }
}
