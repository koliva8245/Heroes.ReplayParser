namespace Heroes.ReplayParser.Replay
{
    public class MapInfo
    {
        /// <summary>
        /// Gets or sets the map name.
        /// </summary>
        public string MapName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the map size.
        /// </summary>
        public Point MapSize { get; set; } = new Point();
    }
}
