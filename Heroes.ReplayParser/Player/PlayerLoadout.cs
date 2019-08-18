namespace Heroes.ReplayParser.Player
{
    public class PlayerLoadout
    {
        /// <summary>
        /// Gets or sets the player's skin / skin tint.
        /// </summary>
        public string SkinAndSkinTint { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's mount / mount tint.
        /// </summary>
        public string MountAndMountTint { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's banner.
        /// </summary>
        public string Banner { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's Spray.
        /// </summary>
        public string Spray { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's announcer pack.
        /// </summary>
        public string AnnouncerPack { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's voice line.
        /// </summary>
        public string VoiceLine { get; set; } = string.Empty;
    }
}
