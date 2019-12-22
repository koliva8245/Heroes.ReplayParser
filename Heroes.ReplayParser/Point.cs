namespace Heroes.ReplayParser
{
    /// <summary>
    /// Contains the properties for point coordinates.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        public int Y { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "{" + X + ", " + Y + "}";
        }
    }
}
