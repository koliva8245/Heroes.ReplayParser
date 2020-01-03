using Heroes.ReplayParser.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Heroes.ReplayParser.Replay
{
    /// <summary>
    /// Contains the properties and methods for the parsed replay.
    /// </summary>
    public partial class StormReplay
    {
        /// <summary>
        /// Gets the latest build that the parser was updated for.
        /// </summary>
        public static int LatestUpdatedBuild => 73016;

        /// <summary>
        /// Gets the value indicating if there is at least one observer.
        /// </summary>
        public bool HasObservers => ClientListByUserID.Any(x => x?.PlayerType == PlayerType.Observer);

        /// <summary>
        /// Gets the value indicating if there is at least one AI.
        /// </summary>
        public bool HasAI => Players.Any(x => x?.PlayerType == PlayerType.Computer);

        /// <summary>
        /// Gets or sets the version of the replay.
        /// </summary>
        public ReplayVersion ReplayVersion { get; set; } = new ReplayVersion();

        /// <summary>
        /// Gets the build number of the replay.
        /// </summary>
        public int ReplayBuild => ReplayVersion.BaseBuild;

        /// <summary>
        /// Gets or sets the total number of elapsed game loops / frames.
        /// </summary>
        public int ElapsedGamesLoops { get; set; }

        /// <summary>
        /// Gets the length of the replay.
        /// </summary>
        public TimeSpan ReplayLength => new TimeSpan(0, 0, ElapsedGamesLoops / 16);

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
        public GameMode GameMode { get; set; } = GameMode.TryMe;

        /// <summary>
        /// Gets or sets the team size of the selected game type.
        /// </summary>
        public string TeamSize { get; set; } = string.Empty;

        /// <summary>
        /// Gets the speed the game was played at.
        /// </summary>
        public GameSpeed GameSpeed { get; set; } = GameSpeed.Unknown;

        /// <summary>
        /// Gets a collection of playing players (no observers, has AI).
        /// </summary>
        public IEnumerable<StormPlayer> StormPlayers => Players;

        /// <summary>
        /// Gets a collection of players (contains observers, no AI).
        /// </summary>
        public IEnumerable<StormPlayer> StormPlayersWithObservers => ClientListByUserID;

        /// <summary>
        /// Gets a collection of observer players.
        /// </summary>
        public IEnumerable<StormPlayer> StormObservers => ClientListByUserID.Where(x => x?.PlayerType == PlayerType.Observer);

        /// <summary>
        /// Gets the total number of playing players (includes AI). Use <see cref="PlayersWithObserversCount"/> instead to include observers.
        /// </summary>
        public int PlayersCount => Players.Length;

        /// <summary>
        /// Gets the total number of players, including observers, in the game. Does not include AI.
        /// </summary>
        public int PlayersWithObserversCount => ClientListByUserID.Length;

        /// <summary>
        /// Gets the total number of observers in the game.
        /// </summary>
        public int PlayersObserversCount => StormObservers.Count();

        /// <summary>
        /// Gets a collection of tracker events.
        /// </summary>
        public IEnumerable<TrackerEvent> TrackerEvents => TrackerEventsInternal;

        /// <summary>
        /// Gets or sets the list of all players (no observers).
        /// </summary>
        /// <remarks>Contains AI.</remarks>
        internal StormPlayer[] Players { get; set; } = new StormPlayer[10];

        /// <summary>
        /// Gets the list of all players connected to the game, using 'm_userId' as index.
        /// </summary>
        /// <remarks>Contains observers. No AI.</remarks>
        internal StormPlayer[] ClientListByUserID { get; private set; } = new StormPlayer[16];

        /// <summary>
        /// Gets the list of all players connected to the game, using 'm_workingSetSlotId' as index.
        /// </summary>
        /// <remarks>Contains AI. No observers.</remarks>
        internal StormPlayer[] ClientListByWorkingSetSlotID { get; private set; } = new StormPlayer[16];

        /// <summary>
        /// In some places, this is used instead of the 'Player' array, in games with less than 10 players.
        /// </summary>
        /// <remarks>Contains AI. No observers.</remarks>
        internal StormPlayer[] PlayersWithOpenSlots { get; private set; } = new StormPlayer[10];

        internal string?[][] TeamHeroAttributeIdBans { get; set; } = new string?[2][] { new string?[3] { null, null, null }, new string?[3] { null, null, null } };

        internal List<TrackerEvent> TrackerEventsInternal { get; set; } = new List<TrackerEvent>();
    }
}
