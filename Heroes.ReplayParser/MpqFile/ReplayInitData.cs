using Heroes.MpqTool;
using Heroes.ReplayParser.Decoders;
using Heroes.ReplayParser.Player;
using Heroes.ReplayParser.Replay;
using System;
using System.Text;

namespace Heroes.ReplayParser.MpqFile
{
    internal class ReplayInitData : IMpqParsable
    {
        public ReplayInitData()
        {
        }

        public string FileName { get; } = "replay.initData";

        public void Parse(StormReplay replay, MpqBuffer mpqBuffer)
        {
            BitPackedDecoder decoder = new BitPackedDecoder(mpqBuffer);

            /* m_userInitialData section */

            uint playerListLength = decoder.ReadBits(5);

            for (int i = 0; i < playerListLength; i++)
            {
                string name = decoder.ReadString(8); // m_name

                if (decoder.ReadBoolean())
                    decoder.ReadString(8); // m_clanTag

                if (decoder.ReadBoolean())
                    decoder.ReadString(40); // m_clanLogo

                if (decoder.ReadBoolean())
                    decoder.ReadBits(8); // m_highestLeague

                if (decoder.ReadBoolean())
                    decoder.ReadBits(32); // m_combinedRaceLevels

                decoder.ReadUInt32(); // m_randomSeed (So far, always 0 in Heroes)

                if (decoder.ReadBoolean())
                    decoder.ReadBits(8); // m_racePreference

                if (decoder.ReadBoolean())
                    decoder.ReadBits(8); // m_teamPreference

                decoder.ReadBoolean(); // m_testMap
                decoder.ReadBoolean(); // m_testAuto
                decoder.ReadBoolean(); // m_examine
                decoder.ReadBoolean(); // m_customInterface

                decoder.ReadUInt32(); // m_testType

                decoder.ReadBits(2); // m_observe

                decoder.ReadString(9); // m_hero - Currently Empty String
                decoder.ReadString(9); // m_skin - Currently Empty String
                decoder.ReadString(9); // m_mount - Currently Empty String

                if (replay.ReplayVersion.Major >= 2)
                {
                    decoder.ReadString(9); // m_banner - Currently Empty String
                    decoder.ReadString(9); // m_spray - Currently Empty String
                }

                decoder.ReadString(7); // m_toonHandle - Currently Empty String

                if (replay.StormPlayersByUserId.TryGetValue(i, out StormPlayer? stormPlayer))
                {
                    if (string.IsNullOrEmpty(stormPlayer.Name))
                        stormPlayer.Name = name;
                }
                else
                {
                    replay.StormPlayersByUserId.Add(i, new StormPlayer()
                    {
                        Name = name,
                    });
                }
            }

            /* m_gameDescription section */

            replay.RandomValue = decoder.ReadBits(32); // m_randomValue

            decoder.ReadString(10); // m_gameCacheName - "Dflt"

            // m_gameOptions
            decoder.ReadBoolean(); // m_lockTeams
            decoder.ReadBoolean(); // m_teamsTogether
            decoder.ReadBoolean(); // m_advancedSharedControl
            decoder.ReadBoolean(); // m_randomRaces
            decoder.ReadBoolean(); // m_battleNet
            decoder.ReadBoolean(); // m_amm
            decoder.ReadBoolean(); // m_competitive
            decoder.ReadBoolean(); // m_practice
            decoder.ReadBoolean(); // m_cooperative
            decoder.ReadBoolean(); // m_noVictoryOrDefeat
            decoder.ReadBoolean(); // m_heroDuplicatesAllowed
            decoder.ReadBits(2); // m_fog
            decoder.ReadBits(2); // m_observers
            decoder.ReadBits(2); // m_userDifficulty
            decoder.ReadUInt64(); // m_clientDebugFlags

            if (replay.ReplayBuild >= 43905 && decoder.ReadBoolean())
            {
                replay.GameMode = decoder.ReadUInt32() switch // m_ammId
                {
                    50001 => GameMode.QuickMatch,
                    50021 => GameMode.Cooperative,
                    50031 => GameMode.Brawl,
                    50041 => GameMode.Practice,
                    50051 => GameMode.UnrankedDraft,
                    50061 => GameMode.HeroLeague,
                    50071 => GameMode.TeamLeague,
                    50091 => GameMode.StormLeague,

                    _ => GameMode.Unknown,
                };
            }

            decoder.ReadBits(3); // m_gameSpeed
            decoder.ReadBits(3); // m_gameType

            if (decoder.ReadBits(5) < 10 && replay.GameMode != GameMode.Brawl) // m_maxUsers
                replay.GameMode = GameMode.TryMe; // or it could be a custom

            decoder.ReadBits(5); // m_maxObservers
            decoder.ReadBits(5); // m_maxPlayers
            decoder.ReadBits(4); // m_maxTeams
            decoder.ReadBits(6); // m_maxColors
            decoder.ReadBits(8); // m_maxRaces

            // Max Controls
            if (replay.ReplayBuild < 59279) // m_maxControls
                decoder.ReadBits(8);
            else
                decoder.ReadBits(4);

            replay.MapInfo.MapSize = new Point
            {
                X = (int)decoder.ReadBits(8), // m_mapSizeX
                Y = (int)decoder.ReadBits(8), // m_mapSizeY
            };

            if (replay.ReplayBuild < 39595)
                return;

            decoder.ReadBits(32); // m_mapFileSyncChecksum
            decoder.ReadString(11); // m_mapFileName
            decoder.ReadString(8); // m_mapAuthorName
            decoder.ReadBits(32); // m_modFileSyncChecksum

            // m_slotDescriptions
            uint slotDescriptionLength = decoder.ReadBits(5);
            for (int i = 0; i < slotDescriptionLength; i++)
            {
                decoder.ReadBitArray((int)decoder.ReadBits(6)); // m_allowedColors
                decoder.ReadBitArray((int)decoder.ReadBits(8)); // m_allowedRaces
                decoder.ReadBitArray((int)decoder.ReadBits(6)); // m_allowedDifficulty

                // m_allowedControls
                if (replay.ReplayBuild < 59279)
                    decoder.ReadBitArray((int)decoder.ReadBits(8));
                else
                    decoder.ReadBitArray((int)decoder.ReadBits(4));

                decoder.ReadBitArray((int)decoder.ReadBits(2)); // m_allowedObserveTypes
                decoder.ReadBitArray((int)decoder.ReadBits(7)); // m_allowedAIBuilds
            }

            decoder.ReadBits(6); // m_defaultDifficulty
            decoder.ReadBits(7); // m_defaultAIBuild

            // m_cacheHandles
            uint cacheHandlesLength = decoder.ReadBits(6);
            for (int i = 0; i < cacheHandlesLength; i++)
                decoder.ReadBytes(40);

            decoder.ReadBoolean(); // m_hasExtensionMod
            decoder.ReadBoolean(); // m_isBlizzardMap
            decoder.ReadBoolean(); // m_isPremadeFFA
            decoder.ReadBoolean(); // m_isCoopMode

            /* m_lobbyState section */

            decoder.ReadBits(3); // m_phase
            uint maxUsers = decoder.ReadBits(5); // m_maxUsers
            uint maxObservers = decoder.ReadBits(5); // m_maxObservers

            if (maxUsers + maxObservers != replay.StormPlayersCount)
            {
                throw new StormParseException($"Max users and max observers do not equal the total players count. Max Users: {maxUsers}. " +
                    $"Max Observers: {maxObservers}. " +
                    $"Players Count: {replay.StormPlayersCount}");
            }

            // m_slots
            uint slotsLength = decoder.ReadBits(5);

            for (int i = 0; i < slotsLength; i++)
            {
                int? userId = null;
                int? workingSetSlotID = null;

                decoder.ReadBits(8); // m_control

                if (decoder.ReadBoolean())
                    userId = (int)decoder.ReadBits(4); // m_userId

                decoder.ReadBits(4); // m_teamId

                if (decoder.ReadBoolean())
                    decoder.ReadBits(5); // m_colorPref
                if (decoder.ReadBoolean())
                    decoder.ReadBits(8); // m_racePref

                decoder.ReadBits(6); // m_difficulty
                decoder.ReadBits(7); // m_aiBuild
                decoder.ReadBits(7); // m_handicap

                uint observerStatus = decoder.ReadBits(2); // m_observe

                decoder.ReadBits(32); // m_logoIndex

                string heroId = decoder.ReadString(9); // m_hero (heroId)

                string skinAndSkinTint = decoder.ReadString(9); // m_skin
                string mountAndMountTint = decoder.ReadString(9); // m_mount

                // m_artifacts
                if (replay.ReplayBuild < 65579 || replay.ReplayBuild == 65617 || replay.ReplayBuild == 65654)
                {
                    uint artifactsLength = decoder.ReadBits(4);
                    for (uint j = 0; j < artifactsLength; j++)
                        decoder.ReadString(9);
                }

                if (decoder.ReadBoolean())
                    workingSetSlotID = (int)decoder.ReadBits(8); // m_workingSetSlotId

                if (userId.HasValue && workingSetSlotID.HasValue)
                {
                    // stormPlayer = replay.StormPlayersByUserId[userId.Value];

                    if (replay.StormPlayersByUserId.ContainsKey(userId.Value) && replay.StormPlayersByWorkingSetSlotId.ContainsKey(workingSetSlotID.Value))
                        replay.StormPlayersByUserId[userId.Value] = replay.StormPlayersByWorkingSetSlotId[workingSetSlotID.Value];
                    else
                        replay.StormPlayersByWorkingSetSlotId.Add(workingSetSlotID.Value, replay.StormPlayersByUserId[userId.Value]);

                    if (observerStatus == 2)
                        replay.StormPlayersByUserId[userId.Value].PlayerType = PlayerType.Spectator;

                    replay.StormPlayersByUserId[userId.Value].PlayerHero.HeroId = heroId;
                    replay.StormPlayersByUserId[userId.Value].PlayerLoadout.SkinAndSkinTint = skinAndSkinTint;
                    replay.StormPlayersByUserId[userId.Value].PlayerLoadout.MountAndMountTint = mountAndMountTint;
                }

                // m_rewards
                uint rewardsLength = decoder.ReadBits(17);
                for (uint j = 0; j < rewardsLength; j++)
                    decoder.ReadBits(32);

                decoder.ReadString(7); // m_toonHandle

                // m_licenses
                if (replay.ReplayBuild < 49582 || replay.ReplayBuild == 49838)
                {
                    uint licensesLength = decoder.ReadBits(9);
                    for (uint j = 0; j < licensesLength; j++)
                        decoder.ReadBits(32);
                }

                if (decoder.ReadBoolean())
                    decoder.ReadBits(4); // m_tandemLeaderUserId

                if (replay.ReplayBuild <= 41504)
                {
                    decoder.ReadStringAsSpan(9); // m_commander - Empty string

                    decoder.ReadBits(32); // m_commanderLevel - So far, always 0
                }

                if (decoder.ReadBoolean() && userId.HasValue) // m_hasSilencePenalty
                    replay.StormPlayersByUserId[userId.Value].IsSilenced = true;

                if (replay.ReplayBuild >= 61718 && decoder.ReadBoolean() && userId.HasValue) // m_hasVoiceSilencePenalty
                    replay.StormPlayersByUserId[userId.Value].IsVoiceSilenced = true;

                if (replay.ReplayBuild >= 66977 && decoder.ReadBoolean() && userId.HasValue) // m_isBlizzardStaff
                    replay.StormPlayersByUserId[userId.Value].IsBlizzardStaff = true;

                if (replay.ReplayBuild >= 69947 && decoder.ReadBoolean() && userId.HasValue) // m_hasActiveBoost
                    replay.StormPlayersByUserId[userId.Value].HasActiveBoost = true;

                if (replay.ReplayVersion.Major >= 2)
                {
                    string banner = decoder.ReadString(9); // m_banner
                    if (userId.HasValue)
                        replay.StormPlayersByUserId[userId.Value].PlayerLoadout.Banner = banner;

                    string spray = decoder.ReadString(9); // m_spray
                    if (userId.HasValue)
                        replay.StormPlayersByUserId[userId.Value].PlayerLoadout.Spray = spray;

                    string announcer = decoder.ReadString(9); // m_announcerPack
                    if (userId.HasValue)
                        replay.StormPlayersByUserId[userId.Value].PlayerLoadout.AnnouncerPack = announcer;

                    string voiceLine = decoder.ReadString(9); // m_voiceLine
                    if (userId.HasValue)
                        replay.StormPlayersByUserId[userId.Value].PlayerLoadout.VoiceLine = voiceLine;

                    // m_heroMasteryTiers
                    if (replay.ReplayBuild >= 52561)
                    {
                        uint heroMasteryTiersLength = decoder.ReadBits(10);
                        for (int j = 0; j < heroMasteryTiersLength; j++)
                        {
                            byte[] heroBytes = BitConverter.GetBytes(decoder.ReadBits(32)); // m_hero
                            Array.Reverse(heroBytes);

                            string heroAttributeName = Encoding.UTF8.GetString(heroBytes);
                            int tier = (int)decoder.ReadBits(8); // m_tier

                            if (userId.HasValue)
                            {
                                replay.StormPlayersByUserId[userId.Value].HeroMasteryTiers.Add(new HeroMasteryTier()
                                {
                                    HeroAttributeId = heroAttributeName,
                                    TierLevel = tier,
                                });
                            }
                        }
                    }
                }
            }

            if (decoder.ReadBits(32) != replay.RandomValue) // m_randomSeed
                throw new StormParseException("Random seed values in replayInitData did not match.");

            if (decoder.ReadBoolean())
                decoder.ReadBits(4); // m_hostUserId

            decoder.ReadBoolean(); // m_isSinglePlayer
            decoder.ReadBits(8); // m_pickedMapTag - So far, always 0
            decoder.ReadBits(32); // m_gameDuration - So far, always 0
            decoder.ReadBits(6); // m_defaultDifficulty
            decoder.ReadBits(7); // m_defaultAIBuild
        }
    }
}
