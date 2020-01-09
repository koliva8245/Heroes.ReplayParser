namespace Heroes.ReplayParser.Replay
{
    /// <summary>
    /// Contains the properties for map information.
    /// </summary>
    public class MapInfo
    {
        /// <summary>
        /// Gets or sets the map name.
        /// </summary>
        public string MapName { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets or sets the map size.
        /// </summary>
        public Point MapSize { get; internal set; }
    }
}
