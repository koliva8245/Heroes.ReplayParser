﻿using Heroes.ReplayParser.Player;
using System;
using System.Collections.Generic;

namespace Heroes.ReplayParser.Replay
{
    public class StormReplay
    {
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
        public long ElapsedGamesLoops { get; set; }

        /// <summary>
        /// Gets or sets the map info.
        /// </summary>
        public MapInfo MapInfo { get; set; } = new MapInfo();

        /// <summary>
        /// Gets or sets the date and time of the when the replay was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the random value.
        /// </summary>
        public long RandomValue { get; set; }

        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
        public GameMode GameMode { get; set; }

        /// <summary>
        /// Gets or sets the team size of the selected game type.
        /// </summary>
        public string TeamSize { get; set; } = string.Empty;

        /// <summary>
        /// Gets the speed the game was played at.
        /// </summary>
        public GameSpeed GameSpeed { get; set; }

        /// <summary>
        /// Gets a collection of playing players (no observers).
        /// </summary>
        public IEnumerable<StormPlayer> StormPlayers => Players;

        /// <summary>
        /// Gets the total number of playing players. Use <see cref="PlayersWithObserversCount"/> to include observers.
        /// </summary>
        public int PlayersCount => Players.Length;

        /// <summary>
        /// Gets the total number of players, including observers, in the game.
        /// </summary>
        public int PlayersWithObserversCount => ClientListByUserID.Length;

        /// <summary>
        /// Gets the list of all players (no observers).
        /// </summary>
        internal StormPlayer[] Players { get; set; } = new StormPlayer[10];

        /// <summary>
        /// Gets the list of all players connected to the game, using 'm_userId' as index.
        /// </summary>
        internal StormPlayer[] ClientListByUserID { get; private set; } = new StormPlayer[16];

        /// <summary>
        /// Gets the list of all players connected to the game, using 'm_workingSetSlotId' as index.
        /// </summary>
        internal StormPlayer[] ClientListByWorkingSetSlotID { get; private set; } = new StormPlayer[16];

        /// <summary>
        /// In some places, this is used instead of the 'Player' array, in games with less than 10 players.
        /// </summary>
        internal StormPlayer[] PlayersWithOpenSlots { get; private set; } = new StormPlayer[10];

        internal Dictionary<int, List<string>> TeamHeroAttributeIdBans { get; set; } = new Dictionary<int, List<string>>();

        /// <summary>
        /// Gets a collection of a team's bans.
        /// </summary>
        /// <param name="team">The team (0 or 1).</param>
        /// <returns>The collection of bans.</returns>
        public IEnumerable<string> GetTeamBans(int team)
        {
            if (TeamHeroAttributeIdBans.TryGetValue(team, out List<string>? values))
                return values;
            else
                return new List<string>();
        }
    }
}
