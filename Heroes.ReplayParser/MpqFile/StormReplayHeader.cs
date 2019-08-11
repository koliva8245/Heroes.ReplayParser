using Heroes.MpqTool;

namespace Heroes.ReplayParser.MpqFile
{
    internal static class StormReplayHeader
    {
        public static void Parse(StormReplay stormReplay, MpqBuffer mpqBuffer)
        {
            mpqBuffer.ReadBytes(3);
            mpqBuffer.ReadByte();
            mpqBuffer.ReadBytes(4); // Data Max Size
            mpqBuffer.ReadBytes(4); // Header Offset
            mpqBuffer.ReadBytes(4); // User Data Header Size

            TrackerEventStructure headerStructure = new TrackerEventStructure(mpqBuffer);

            // headerStructure.StructureByIndex[0].GetValueAsString(); // m_signature => "Heroes of the Storm replay 11

            // m_version struct
            stormReplay.ReplayVersion.Major = headerStructure.StructureByIndex?[1].StructureByIndex?[1].GetValueAsInt32() ?? 0; // m_major
            stormReplay.ReplayVersion.Minor = headerStructure.StructureByIndex?[1].StructureByIndex?[2].GetValueAsInt32() ?? 0; // m_minor
            stormReplay.ReplayVersion.Revision = headerStructure.StructureByIndex?[1].StructureByIndex?[3].GetValueAsInt32() ?? 0; // m_revision
            stormReplay.ReplayVersion.Build = headerStructure.StructureByIndex?[1].StructureByIndex?[4].GetValueAsInt32() ?? 0; // m_build
            stormReplay.ReplayVersion.BaseBuild = headerStructure.StructureByIndex?[1].StructureByIndex?[5].GetValueAsInt32() ?? 0; // m_baseBuild

            // the major version is a 0 before build 51978, it may be set a 1
            /* if (stormReplay.ReplayBuild < 51978)
                stormReplay.ReplayVersion.Major = 1; */

            /* headerStructure.StructureByIndex[2].GetValueAsUInt32(); m_type */

            stormReplay.ElapsedGamesLoops = headerStructure.StructureByIndex?[3].GetValueAsInt32() ?? 0; // m_elapsedGameLoops

            /* headerStructure.StructureByIndex?[4].GetValueAsInt32(); // m_useScaledTime */
            /* headerStructure.StructureByIndex?[5].GetValueAsInt32(); // m_ngdpRootKey */

            if (stormReplay.ReplayBuild >= 39951)
                stormReplay.ReplayVersion.BaseBuild = headerStructure.StructureByIndex?[6].GetValueAsInt32() ?? 0; // m_dataBuildNum

            /* headerStructure.StructureByIndex?[7].GetValueAsInt32(); // replayCompatibilityHash */
        }
    }
}
