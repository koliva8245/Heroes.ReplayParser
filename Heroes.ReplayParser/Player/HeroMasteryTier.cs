namespace Heroes.ReplayParser.Player
{
    public class HeroMasteryTier
    {
        public string HeroAttributeId { get; set; } = string.Empty;
        public int TierLevel { get; set; }

        public override string ToString()
        {
            return $"{HeroAttributeId}: {TierLevel}";
        }
    }
}
