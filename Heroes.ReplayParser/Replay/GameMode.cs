namespace Heroes.ReplayParser.Replay
{
    public enum GameMode
    {
        Unknown = 0,
        Event = 1 << 0,
        Custom = 1 << 1,
        TryMe = 1 << 2,
        Practice = 1 << 3,
        Cooperative = 1 << 4,
        QuickMatch = 1 << 5,
        HeroLeague = 1 << 6,
        TeamLeague = 1 << 7,
        UnrankedDraft = 1 << 8,
        Brawl = 1 << 9,
        StormLeague = 1 << 10,
    }
}
