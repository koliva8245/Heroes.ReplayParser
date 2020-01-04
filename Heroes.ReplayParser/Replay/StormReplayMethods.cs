using System;
using System.Collections.Generic;
using System.Linq;

namespace Heroes.ReplayParser.Replay
{
    public partial class StormReplay
    {
        private readonly Lazy<Dictionary<int, TeamLevel>> _teamBlueLevels = new Lazy<Dictionary<int, TeamLevel>>();
        private readonly Lazy<Dictionary<int, TeamLevel>> _teamRedLevels = new Lazy<Dictionary<int, TeamLevel>>();

        /// <summary>
        /// Gets a collection of a team's bans.
        /// </summary>
        /// <param name="stormTeam">The team.</param>
        /// <returns>The collection of bans.</returns>
        public IEnumerable<string?> GetTeamBans(StormTeam stormTeam)
        {
            if (!(stormTeam == StormTeam.Blue || stormTeam == StormTeam.Red))
                return Enumerable.Empty<string>();

            return TeamHeroAttributeIdBans[(int)stormTeam];
        }

        /// <summary>
        /// Gets a collection of the draft order.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DraftPick> GetDraftOrder()
        {
            foreach (TrackerEvent trackerEvent in TrackerEventsInternal.Where(x =>
                x.TrackerEventType == TrackerEventType.HeroBannedEvent ||
                x.TrackerEventType == TrackerEventType.HeroPickedEvent ||
                x.TrackerEventType == TrackerEventType.HeroSwappedEvent))
            {
                switch (trackerEvent.TrackerEventType)
                {
                    case TrackerEventType.HeroBannedEvent:
                        yield return new DraftPick()
                        {
                           HeroSelected = trackerEvent.VersionedDecoder!.StructureByIndex![0].GetValueAsString(),
                           SelectedPlayerSlotId = (int)trackerEvent.VersionedDecoder.StructureByIndex[1].GetValueAsUInt32(),
                           PickType = DraftPickType.Banned,
                        };
                        break;

                    case TrackerEventType.HeroPickedEvent:
                        yield return new DraftPick()
                        {
                            HeroSelected = trackerEvent.VersionedDecoder!.StructureByIndex![0].GetValueAsString(),
                            SelectedPlayerSlotId = (int)trackerEvent.VersionedDecoder.StructureByIndex[1].GetValueAsUInt32(),
                            PickType = DraftPickType.Picked,
                        };
                        break;

                    case TrackerEventType.HeroSwappedEvent:
                        yield return new DraftPick()
                        {
                            HeroSelected = trackerEvent.VersionedDecoder!.StructureByIndex![0].GetValueAsString(),
                            SelectedPlayerSlotId = (int)trackerEvent.VersionedDecoder.StructureByIndex[1].GetValueAsUInt32(),
                            PickType = DraftPickType.Swapped,
                        };
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a team's final level at the end of the game.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public int GetTeamFinalLevel(StormTeam team)
        {
            if (team == StormTeam.Blue)
            {
                if (!_teamBlueLevels.IsValueCreated)
                    GetTeamLevels(team);

                return _teamBlueLevels.Value.Values.LastOrDefault().Level;
            }
            else if (team == StormTeam.Red)
            {
                if (!_teamRedLevels.IsValueCreated)
                    GetTeamLevels(team);

                return _teamRedLevels.Value.Values.LastOrDefault().Level;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a collection of a <see cref="StormTeam"/>'s levels.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public IEnumerable<TeamLevel> GetTeamLevels(StormTeam team)
        {
            if (team == StormTeam.Blue && _teamBlueLevels.IsValueCreated)
                return _teamBlueLevels.Value.Values;
            else if (team == StormTeam.Red && _teamRedLevels.IsValueCreated)
                return _teamRedLevels.Value.Values;

            foreach (TrackerEvent trackerEvent in TrackerEventsInternal.Where(x => x.TrackerEventType == TrackerEventType.StatGameEvent && x.VersionedDecoder?.StructureByIndex?[0].GetValueAsString() == "LevelUp"))
            {
                if (trackerEvent.VersionedDecoder?.StructureByIndex?[2].OptionalData?.ArrayData?[0].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "PlayerID" &&
                    trackerEvent.VersionedDecoder?.StructureByIndex?[2].OptionalData?.ArrayData?[1].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "Level")
                {
                    int playerId = (int)trackerEvent.VersionedDecoder!.StructureByIndex![2].OptionalData!.ArrayData![0].StructureByIndex![1].GetValueAsUInt32();

                    if (PlayersWithOpenSlots[playerId - 1].Team == team)
                    {
                        TeamLevel teamLevel = new TeamLevel()
                        {
                            Level = (int)trackerEvent.VersionedDecoder!.StructureByIndex![2].OptionalData!.ArrayData![1].StructureByIndex![1].GetValueAsUInt32(),
                            Time = trackerEvent.TimeSpan,
                        };

                        if (team == StormTeam.Blue)
                            _teamBlueLevels.Value.TryAdd(teamLevel.Level, teamLevel);
                        else if (team == StormTeam.Red)
                            _teamRedLevels.Value.TryAdd(teamLevel.Level, teamLevel);
                    }
                }
            }

            if (team == StormTeam.Blue)
                return _teamBlueLevels.Value.Values;
            else if (team == StormTeam.Red)
                return _teamRedLevels.Value.Values;
            else
                return Enumerable.Empty<TeamLevel>();
        }

        /// <summary>
        /// Gets a collection of a team's experience breakdown.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public IEnumerable<TeamXPBreakdown> GetTeamXPBreakdown(StormTeam team)
        {
            TeamXPBreakdown teamXPBreakdown = new TeamXPBreakdown();

            foreach (TrackerEvent trackerEvent in TrackerEventsInternal.Where(x => x.TrackerEventType == TrackerEventType.StatGameEvent).ToList())
            {
                string value = trackerEvent.VersionedDecoder!.StructureByIndex![0].GetValueAsString();

                if (value == "PeriodicXPBreakdown")
                {
                    if (team == (StormTeam)(trackerEvent.VersionedDecoder!.StructureByIndex![2].OptionalData!.ArrayData![0].StructureByIndex![1].GetValueAsUInt32() - 1) &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[2].OptionalData?.ArrayData?[1].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "TeamLevel" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[0].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "GameTime" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[1].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "PreviousGameTime" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[2].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "MinionXP" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[3].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "CreepXP" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[4].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "StructureXP" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[5].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "HeroXP" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[6].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "TrickleXP")
                    {
                        teamXPBreakdown = new TeamXPBreakdown()
                        {
                            Level = (int)trackerEvent.VersionedDecoder!.StructureByIndex![2].OptionalData!.ArrayData![1].StructureByIndex![1].GetValueAsUInt32(),
                            Time = trackerEvent.TimeSpan,
                            MinionXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![2].StructureByIndex![1].GetValueAsInt64() / 4096),
                            MercenaryXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![3].StructureByIndex![1].GetValueAsInt64() / 4096),
                            StructureXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![4].StructureByIndex![1].GetValueAsInt64() / 4096),
                            HeroXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![5].StructureByIndex![1].GetValueAsInt64() / 4096),
                            PassiveXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![6].StructureByIndex![1].GetValueAsInt64() / 4096),
                        };

                        yield return teamXPBreakdown;
                    }
                }
                else if (value == "EndOfGameXPBreakdown")
                {
                    int playerId = (int)trackerEvent.VersionedDecoder!.StructureByIndex![2].OptionalData!.ArrayData![0].StructureByIndex![1].GetValueAsUInt32();

                    if (PlayersWithOpenSlots[playerId - 1].Team == team &&
                        teamXPBreakdown.Time != trackerEvent.TimeSpan &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[2].OptionalData?.ArrayData?[0].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "PlayerID" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[0].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "MinionXP" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[1].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "CreepXP" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[2].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "StructureXP" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[3].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "HeroXP" &&
                        trackerEvent.VersionedDecoder?.StructureByIndex?[3].OptionalData?.ArrayData?[4].StructureByIndex?[0].StructureByIndex?[0].GetValueAsString() == "TrickleXP")
                    {
                        teamXPBreakdown = new TeamXPBreakdown()
                        {
                            Level = GetTeamFinalLevel(team),
                            Time = trackerEvent.TimeSpan,
                            MinionXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![0].StructureByIndex![1].GetValueAsInt64() / 4096),
                            MercenaryXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![1].StructureByIndex![1].GetValueAsInt64() / 4096),
                            StructureXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![2].StructureByIndex![1].GetValueAsInt64() / 4096),
                            HeroXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![3].StructureByIndex![1].GetValueAsInt64() / 4096),
                            PassiveXP = (int)(trackerEvent.VersionedDecoder!.StructureByIndex![3].OptionalData!.ArrayData![4].StructureByIndex![1].GetValueAsInt64() / 4096),
                        };

                        yield return teamXPBreakdown;
                    }
                }
            }
        }
    }
}
