using System.Collections.Generic;
using System.Linq;

namespace Heroes.ReplayParser.Replay
{
    public partial class StormReplay
    {
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
        /// Gets a collection of a <see cref="StormTeam"/>'s levels.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public IEnumerable<TeamLevel> GetTeamLevels(StormTeam team)
        {
            Dictionary<int, TeamLevel> teamLevels = new Dictionary<int, TeamLevel>();

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

                        teamLevels.TryAdd(teamLevel.Level, teamLevel);
                    }
                }
            }

            return teamLevels.Values;
        }
    }
}
