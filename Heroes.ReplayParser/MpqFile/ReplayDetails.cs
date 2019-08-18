using Heroes.MpqTool;
using Heroes.ReplayParser.Decoders;
using Heroes.ReplayParser.Player;
using Heroes.ReplayParser.Replay;
using System;

namespace Heroes.ReplayParser.MpqFile
{
    internal class ReplayDetails : IMpqParsable
    {
        public ReplayDetails()
        {
        }

        public string FileName { get; } = "replay.details";

        public void Parse(StormReplay replay, MpqBuffer mpqBuffer)
        {
            VersionedDecoder versionedDecoder = new VersionedDecoder(mpqBuffer);

            // this section does not include the observers
            VersionedDecoder[]? versionDecoders = versionedDecoder.StructureByIndex?[0].OptionalData?.ArrayData;

            for (int i = 0; i < versionDecoders?.Length; i++)
            {
                StormPlayer stormPlayer = new StormPlayer
                {
                    Name = versionDecoders[i].StructureByIndex?[0].GetValueAsString() ?? string.Empty, // m_name
                };

                stormPlayer.ToonHandle.Region = (int)(versionDecoders[i].StructureByIndex?[1].StructureByIndex?[0].GetValueAsUInt32() ?? 0); // m_region
                stormPlayer.ToonHandle.ProgramId = versionDecoders[i].StructureByIndex?[1].StructureByIndex?[1].GetValueAsUInt32() ?? 0; // m_programId
                stormPlayer.ToonHandle.Realm = (int)(versionDecoders[i].StructureByIndex?[1].StructureByIndex?[2].GetValueAsUInt32() ?? 0); // m_realm

                // [1] // m_name

                stormPlayer.ToonHandle.Id = versionDecoders[i].StructureByIndex?[1].StructureByIndex?[4].GetValueAsUInt32() ?? 0; // m_id

                // [2] // m_race (SC2 Remnant, Always Empty String in Heroes of the Storm)
                // [3]...array // m_color
                // [4] // m_control

                stormPlayer.Team = (int)(versionDecoders[i].StructureByIndex?[5].GetValueAsUInt32() ?? 0); // m_teamId

                // x.StructureByIndex[6] // m_handicap
                // x.StructureByIndex[7] // m_observe

                stormPlayer.IsWinner = versionDecoders[i].StructureByIndex?[8].GetValueAsUInt32() == 1; // m_result
                stormPlayer.WorkingSetSlotId = (int)(versionDecoders[i].StructureByIndex?[9].OptionalData?.GetValueAsUInt32() ?? 0); // m_workingSetSlotId
                stormPlayer.PlayerHero.HeroName = versionDecoders[i].StructureByIndex?[10].GetValueAsString() ?? string.Empty; // m_hero (name)

                replay.StormPlayersByWorkingSetSlotId.Add(stormPlayer.WorkingSetSlotId, stormPlayer);
            }

            replay.MapInfo.MapName = versionedDecoder.StructureByIndex?[1].GetValueAsString() ?? string.Empty;

            // [2] - m_difficulty
            // [3] - m_thumbnail - "Minimap.tga", "CustomMiniMap.tga", etc
            // [4] - m_isBlizzardMap

            replay.Timestamp = DateTime.FromFileTimeUtc(versionedDecoder.StructureByIndex?[5].GetValueAsInt64() ?? 0); // m_timeUTC

            // There was a bug during the below builds where timestamps were buggy for the Mac build of Heroes of the Storm
            // The replay, as well as viewing these replays in the game client, showed years such as 1970, 1999, etc
            // I couldn't find a way to get the correct timestamp, so I am just estimating based on when these builds were live
            if (replay.ReplayBuild == 34053 && replay.Timestamp < new DateTime(2015, 2, 8))
                replay.Timestamp = new DateTime(2015, 2, 13);
            else if (replay.ReplayBuild == 34190 && replay.Timestamp < new DateTime(2015, 2, 15))
                replay.Timestamp = new DateTime(2015, 2, 20);

            // [6] - m_timeLocalOffset - For Windows replays, this is Utc offset.  For Mac replays, this is actually the entire Local Timestamp
            // [7] - m_description - Empty String
            // [8] - m_imageFilePath - Empty String
            // [9] - m_mapFileName - Empty String
            // [10] - m_cacheHandles - "s2ma"
            // [11] - m_miniSave - 0
            // [12] - m_gameSpeed - 4
            // [13] - m_defaultDifficulty - Usually 1 or 7
            // [14] - m_modPaths - Null
            // [15] - m_campaignIndex - 0
            // [16] - m_restartAsTransitionMap - 0
        }
    }
}
