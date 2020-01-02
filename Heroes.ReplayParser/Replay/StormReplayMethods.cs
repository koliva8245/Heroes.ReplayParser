using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
