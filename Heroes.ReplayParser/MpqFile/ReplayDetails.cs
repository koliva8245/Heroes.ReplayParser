using Heroes.MpqTool;
using Heroes.ReplayParser.Player;
using System;
using System.Linq;

namespace Heroes.ReplayParser.MpqFile
{
    internal class ReplayDetails : IMpqParsable
    {
        public ReplayDetails()
        {
        }

        public string FileName { get; } = "replay.details";

        public void Parse(StormReplay stormReplay, MpqBuffer mpqBuffer)
        {
            TrackerEventStructure replayDetailsStructure = new TrackerEventStructure(mpqBuffer);

            stormReplay.AddStormPlayer(replayDetailsStructure.StructureByIndex?[0].OptionalData?.ArrayData.Select(x =>
            {
                StormPlayer stormPlayer = new StormPlayer
                {
                    Name = x.StructureByIndex?[0].GetValueAsString() ?? string.Empty, // m_name
                };

                stormPlayer.ToonHandle.Region = x.StructureByIndex?[1].StructureByIndex?[0].GetValueAsInt32() ?? 0; // m_region
                stormPlayer.ToonHandle.ProgramId = x.StructureByIndex?[1].StructureByIndex?[1].GetValueAsInt32() ?? 0; // m_programId
                stormPlayer.ToonHandle.Realm = x.StructureByIndex?[1].StructureByIndex?[2].GetValueAsInt32() ?? 0; // m_realm

                // x.StructureByIndex?[1] // m_name

                stormPlayer.ToonHandle.Id = x.StructureByIndex?[1].StructureByIndex?[4].GetValueAsInt32() ?? 0; // m_id

                // x.StructureByIndex?[2] // m_race (SC2 Remnant, Always Empty String in Heroes of the Storm)
                // x.StructureByIndex?[3]...array // m_color

                stormPlayer.PlayerType = (PlayerType)(x.StructureByIndex?[4].GetValueAsInt32() ?? 1); // m_control
                stormPlayer.Team = x.StructureByIndex?[5].GetValueAsInt32() ?? 0; // m_teamId

                // x.StructureByIndex[6] // m_handicap
                // x.StructureByIndex[7] // m_observe

                stormPlayer.IsWinner = x.StructureByIndex?[8].GetValueAsInt32() == 1; // m_result
                stormPlayer.WorkingSetSlotId = x.StructureByIndex?[9].OptionalData?.GetValueAsInt32() ?? 0; // m_workingSetSlotId
                stormPlayer.HeroName = x.StructureByIndex?[10].GetValueAsString() ?? string.Empty; // m_hero

                return stormPlayer;
            }));

            stormReplay.MapName = replayDetailsStructure.StructureByIndex?[1].GetValueAsString() ?? string.Empty;

            // [2] - m_difficulty
            // [3] - m_thumbnail - "Minimap.tga", "CustomMiniMap.tga", etc
            // [4] - m_isBlizzardMap

            stormReplay.Timestamp = DateTime.FromFileTimeUtc(replayDetailsStructure.StructureByIndex?[5].GetValueAsInt64() ?? 0); // m_timeUTC

            // There was a bug during the below builds where timestamps were buggy for the Mac build of Heroes of the Storm
            // The replay, as well as viewing these replays in the game client, showed years such as 1970, 1999, etc
            // I couldn't find a way to get the correct timestamp, so I am just estimating based on when these builds were live
            if (stormReplay.ReplayBuild == 34053 && stormReplay.Timestamp < new DateTime(2015, 2, 8))
                stormReplay.Timestamp = new DateTime(2015, 2, 13);
            else if (stormReplay.ReplayBuild == 34190 && stormReplay.Timestamp < new DateTime(2015, 2, 15))
                stormReplay.Timestamp = new DateTime(2015, 2, 20);

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
