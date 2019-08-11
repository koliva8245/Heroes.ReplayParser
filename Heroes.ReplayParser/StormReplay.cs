using Heroes.ReplayParser.Player;
using System;
using System.Collections.Generic;

namespace Heroes.ReplayParser
{
    public class StormReplay
    {
        private readonly List<StormPlayer> _stormPlayers = new List<StormPlayer>();

        /// <summary>
        /// Gets the latest build that the parser was updated for.
        /// </summary>
        public static int LatestUpdatedBuild => 73016;

        /// <summary>
        /// Gets or sets the version of the replay.
        /// </summary>
        public ReplayVersion ReplayVersion { get; set; } = new ReplayVersion();

        /// <summary>
        /// Gets the build number of the replay.
        /// </summary>
        public int ReplayBuild => ReplayVersion.BaseBuild;

        /// <summary>
        /// Gets or sets the total number of elapsed game loops.
        /// </summary>
        public int ElapsedGamesLoops { get; set; }

        /// <summary>
        /// Gets or sets the map name.
        /// </summary>
        public string MapName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time of the when the replay was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        public IEnumerable<StormPlayer> StormPlayers => _stormPlayers;

        internal void AddStormPlayer(IEnumerable<StormPlayer>? stormPlayers)
        {
            if (stormPlayers is null)
            {
                throw new ArgumentNullException(nameof(stormPlayers));
            }

            _stormPlayers.AddRange(stormPlayers);
        }
    }
}
