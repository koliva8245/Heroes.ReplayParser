﻿using Heroes.MpqTool;
using Heroes.ReplayParser.Decoders;
using Heroes.ReplayParser.Replay;

namespace Heroes.ReplayParser.MpqFile
{
    internal static class StormReplayHeader
    {
        public static void Parse(StormReplay replay, MpqBuffer mpqBuffer)
        {
            mpqBuffer.ReadBytes(3);
            mpqBuffer.ReadByte();
            mpqBuffer.ReadBytes(4); // Data Max Size
            mpqBuffer.ReadBytes(4); // Header Offset
            mpqBuffer.ReadBytes(4); // User Data Header Size

            VersionedDecoder versionedDecoder = new VersionedDecoder(mpqBuffer);

            // headerStructure.StructureByIndex[0].GetValueAsString(); // m_signature => "Heroes of the Storm replay 11

            // m_version struct
            replay.ReplayVersion.Major = (int)(versionedDecoder.StructureByIndex?[1].StructureByIndex?[1].GetValueAsUInt32() ?? 0); // m_major
            replay.ReplayVersion.Minor = (int)(versionedDecoder.StructureByIndex?[1].StructureByIndex?[2].GetValueAsUInt32() ?? 0); // m_minor
            replay.ReplayVersion.Revision = (int)(versionedDecoder.StructureByIndex?[1].StructureByIndex?[3].GetValueAsUInt32() ?? 0); // m_revision
            replay.ReplayVersion.Build = (int)(versionedDecoder.StructureByIndex?[1].StructureByIndex?[4].GetValueAsUInt32() ?? 0); // m_build
            replay.ReplayVersion.BaseBuild = (int)(versionedDecoder.StructureByIndex?[1].StructureByIndex?[5].GetValueAsUInt32() ?? 0); // m_baseBuild

            // the major version is a 0 before build 51978, it may be set a 1
            /* if (stormReplay.ReplayBuild < 51978)
                stormReplay.ReplayVersion.Major = 1; */

            /* headerStructure.StructureByIndex[2].GetValueAsUInt32(); m_type */

            replay.ElapsedGamesLoops = versionedDecoder.StructureByIndex?[3].GetValueAsUInt32() ?? 0; // m_elapsedGameLoops

            /* headerStructure.StructureByIndex?[4].GetValueAsInt32(); // m_useScaledTime */
            /* headerStructure.StructureByIndex?[5].GetValueAsInt32(); // m_ngdpRootKey */

            if (replay.ReplayBuild >= 39951)
                replay.ReplayVersion.BaseBuild = (int)(versionedDecoder.StructureByIndex?[6].GetValueAsUInt32() ?? 0); // m_dataBuildNum

            /* headerStructure.StructureByIndex?[7].GetValueAsInt32(); // replayCompatibilityHash */
        }
    }
}
