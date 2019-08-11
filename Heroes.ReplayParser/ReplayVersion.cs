namespace Heroes.ReplayParser
{
    public class ReplayVersion
    {
        /// <summary>
        /// Gets or sets the first number.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Gets or sets the second number.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Gets or sets the third number.
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the fourth number.
        /// </summary>
        public int Build { get; set; }

        /// <summary>
        /// Gets or sets the fifth number.
        /// </summary>
        public int BaseBuild { get; set; }

        public override string? ToString()
        {
            return $"{Major}.{Minor}.{Revision}.{Build}";
        }
    }
}
