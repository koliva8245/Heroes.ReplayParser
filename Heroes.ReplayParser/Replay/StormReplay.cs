using Heroes.ReplayParser.Player;
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
        /// Gets a collection of playing players.
        /// </summary>
        public IEnumerable<StormPlayer> StormPlayers => StormPlayersByUserId.Values;

        /// <summary>
        /// Gets the number of playering players.
        /// </summary>
        public int StormPlayersCount => StormPlayersByUserId.Count;

        //internal void AddStormPlayer(IEnumerable<StormPlayer>? stormPlayers)
        //{
        //    if (stormPlayers is null)
        //    {
        //        throw new ArgumentNullException(nameof(stormPlayers));
        //    }

        //    _stormPlayers.AddRange(stormPlayers);
        //}

        internal Dictionary<int, StormPlayer> StormPlayersByUserId { get; set; } = new Dictionary<int, StormPlayer>();
        internal Dictionary<int, StormPlayer> StormPlayersByWorkingSetSlotId { get; set; } = new Dictionary<int, StormPlayer>();

        //internal void AddStormPlayerByUserId(uint userId, StormPlayer stormPlayer)
        //{
        //    if (userId < 0)
        //        throw new ArgumentOutOfRangeException(nameof(userId));
        //    if (stormPlayer is null)
        //        throw new ArgumentNullException(nameof(stormPlayer));

        //    _stormPlayersByUserId.Add(userId, stormPlayer);
        //}

        //internal void AddStormPlayerByWorkingSetSlotId(uint workSetSlotId, StormPlayer stormPlayer)
        //{
        //    if (workSetSlotId < 0)
        //        throw new ArgumentOutOfRangeException(nameof(workSetSlotId));
        //    if (stormPlayer is null)
        //        throw new ArgumentNullException(nameof(stormPlayer));

        //    _stormPlayersByWorkingSetSlotId.Add(workSetSlotId, stormPlayer);
        //}

        //internal bool PlayerByUserIdExists
        //internal StormPlayer GetPlayerByUserId(uint userId)
        //{
        //    if (_stormPlayersByUserId.TryGetValue(userId, out StormPlayer? value))
        //        return value;
        //    else
        //        throw new IndexOutOfRangeException(nameof(userId));
        //}

        //internal void AddClientStormByWorkingSetSlotIdPlayer(StormPlayer stormPlayer)
        //{
        //    if (stormPlayer is null)
        //    {
        //        throw new ArgumentNullException(nameof(stormPlayer));
        //    }

        //    _clientListByWorkingSetSlotIdStormPlayers.Add(stormPlayer);
        //}
    }
}
