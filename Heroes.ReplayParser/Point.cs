namespace Heroes.ReplayParser
{
    /// <summary>
    /// Contains the properties for point coordinates.
    /// </summary>
    public struct Point
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The X-coordinate.</param>
        /// <param name="y">The Y-coordinate.</param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        public double Y { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "{" + X + ", " + Y + "}";
        }
    }
}
