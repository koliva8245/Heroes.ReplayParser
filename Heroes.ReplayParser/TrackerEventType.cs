namespace Heroes.ReplayParser
{
    public enum TrackerEventType
    {
        UnitBornEvent = 1,
        /* UnitID Index, UnitID Recycle, Unit Type Name, PlayerID with Control, PlayerID with Upkeep, X, Y */

        UnitDiedEvent = 2,
        /* UnitID Index, UnitID Recycle, PlayerID that Killed This, X, Y, Killing UnitID Index, Killing UnitID Recycle */

        UnitOwnerChangeEvent = 3,
        /* UnitID Index, UnitID Recycle, New PlayerID with Control, New PlayerID with Upkeep */

        UnitTypeChangeEvent = 4,
        /* UnitID Index, UnitID Recycle, New Unit Type Name */

        UpgradeEvent = 5,
        /* PlayerID, Upgrade Type Name, Count */

        UnitInitEvent = 6,
        /* UnitID, Unit Type Name, PlayerID with Control, PlayerID with Upkeep, X, Y */

        UnitDoneEvent = 7,
        /* UnitID */

        UnitPositionsEvent = 8,
        /* First UnitID Index, Items Array (UnitID Index Offset, X, Y) */

        PlayerSetupEvent = 9,
        /* PlayerID, Player Type (1=Human, 2=CPU, 3=Neutral, 4=Hostile), UserID, SlotID */

        StatGameEvent = 10,
        /* EventName, StringData, InitData, FixedData */

        ScoreResultEvent = 11,
        /* InstanceList (20+ length array of Name/Value pairs) */

        UnitRevivedEvent = 12,
        /* UnitID, X, Y */

        HeroBannedEvent = 13,
        /* Hero, ControllingTeam */

        HeroPickedEvent = 14,
        /* Hero, ControllingPlayer */

        HeroSwappedEvent = 15,
        /* Hero, NewControllingPlayer */
    }
}
