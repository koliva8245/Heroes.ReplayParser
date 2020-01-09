using System;

namespace Heroes.ReplayParser.Replay
{
    /// <summary>
    /// Contains the properties for a team level.
    /// </summary>
    public class TeamLevel
    {
        /// <summary>
        /// Gets or sets the level of the team.
        /// </summary>
        public int Level { get; internal set; }

        /// <summary>
        /// Gets or sets the time that the team leveled up.
        /// </summary>
        public TimeSpan Time { get; internal set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Level {Level}: {Time}";
        }
    }
}
