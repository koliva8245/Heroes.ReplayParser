using Heroes.MpqToolV2;
using Heroes.ReplayParser.Decoders;
using Heroes.ReplayParser.Replay;
using System;

namespace Heroes.ReplayParser.MpqFiles
{
    internal static class ReplayServerBattlelobby
    {
        private static string ExceptionHeader = "battlelobby";

        public static string FileName { get; } = "replay.server.battlelobby";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader.ResetIndex();
            BitReader.EndianType = EndianType.BigEndian;

            uint dependenciesLength = source.ReadBits(6);
            for (int i = 0; i < dependenciesLength; i++)
            {
                source.ReadBlobAsString(10);
            }

            // s2ma cache handles
            uint s2maCacheHandlesLength = source.ReadBits(6);
            for (int i = 0; i < s2maCacheHandlesLength; i++)
            {
                if (source.ReadStringFromBytes(4) != "s2ma")
                    throw new StormParseException($"{ExceptionHeader}: s2ma cache");

                source.ReadAlignedBytes(36);
            }

            // we're just going to skip all the way down to the s2mh

            BitReader.AlignToByte();

            for (; ;)
            {
                if (source.ReadStringFromBytes(4) == "s2mh")
                {
                    BitReader.Index -= 4;
                    break;
                }
                else
                {
                    BitReader.Index -= 3;
                }
            }

            // source.ReadBits(???); // this depends on previous data (not byte aligned)

            // s2mh cache handles
            // uint s2mhCacheHandlesLength = source.ReadBits(6);
            // for (int i = 0; i < s2mhCacheHandlesLength; i++)
            for (int i = 0; i < s2maCacheHandlesLength; i++) // temp
            {
                if (source.ReadStringFromBytes(4) != "s2mh")
                    throw new StormParseException($"{ExceptionHeader}: s2mh cache");

                source.ReadAlignedBytes(36);
            }

            uint collectionSize;

            // player collections

            // strings gone starting with build (ptr) 55929
            if (replay.ReplayBuild >= 48027)
                collectionSize = source.ReadBits(16);
            else
                collectionSize = source.ReadBits(32);

            for (uint i = 0; i < collectionSize; i++)
            {
                if (replay.ReplayBuild >= 55929)
                    source.ReadAlignedBytes(8); // most likey an identifier for the item; first six bytes are 0x00
                else
                    source.ReadStringFromBytes(source.ReadAlignedByte());
            }

            // use to determine if the collection item is usable by the player (owns/free to play/internet cafe)
            if (source.ReadBits(32) != collectionSize)
                throw new StormParseException($"{ExceptionHeader}: collection difference");

            for (int i = 0; i < collectionSize; i++)
            {
                for (int j = 0; j < 16; j++) // 16 is total player slots
                {
                    source.ReadAlignedByte();
                    source.ReadAlignedByte(); // more likely a boolean to get the value

                    if (replay.ReplayBuild < 55929)
                    {
                        // when the identifier is a string, set the value to the appropriate array index
                    }
                }
            }

            // Player info

            //if (replay.ReplayBuild <= 43259 || replay.ReplayBuild == 47801)
            //{
            //    // Builds that are not yet supported for detailed parsing
            //    // build 47801 is a ptr build that had new data in the battletag section, the data was changed in 47944 (patch for 47801)
            //    //GetBattleTags(replay, bitReader);
            //    return;
            //}

            replay.RandomValue = source.ReadBits(32); // m_randomSeed

            source.ReadAlignedBytes(4);

            uint playerListLength = source.ReadBits(5);

            if (replay.PlayersWithObserversCount != playerListLength)
                throw new StormParseException($"{ExceptionHeader}: mismatch on player list length - {playerListLength} to {replay.PlayersWithObserversCount}");

            for (uint i = 0; i < playerListLength; i++)
            {
                source.ReadBits(3);
                source.ReadUnalignedBytes(24);
                source.ReadBits(24);
                source.ReadBits(16);
                source.ReadBits(10);

                int idLength = (int)source.ReadBits(7);
                if (source.ReadStringFromBytes(2) != "T:")
                    throw new StormParseException($"{ExceptionHeader}: Not T:");

                replay.ClientListByUserID[i].BattleTID = source.ReadStringFromBytes(idLength);

                source.ReadAlignedBytes(4); // same for all players (most of the time)

                if (replay.ReplayVersion.Build <= 47479)
                {
                    source.ReadAlignedBytes(5);
                    source.ReadBits(5);

                    idLength = (int)source.ReadBits(7);
                    if (source.ReadStringFromBytes(2) != "T:")
                        throw new StormParseException($"{ExceptionHeader}: Not T:");

                    if (replay.ClientListByUserID[i].BattleTID != source.ReadStringFromBytes(idLength))
                        throw new StormParseException($"{ExceptionHeader}: Duplicate TID does not match");
                }
                else
                {
                    source.ReadAlignedBytes(25);
                }

                // source.ReadAlignedBytes(8); ai games have 8 more bytes somewhere around here

                source.ReadBits(7);

                if (!source.ReadBoolean())
                {
                    // repeat of the collection section above
                    if (replay.ReplayBuild >= 51609)
                    {
                        uint size = source.ReadBits(12);

                        int bytesSize = (int)(size / 8);
                        int bitsSize = (int)(size % 8);

                        source.ReadUnalignedBytes(bytesSize);
                        source.ReadBits(bitsSize);

                        source.ReadBoolean();
                    }
                    //else
                    //{
                    //    if (replay.ReplayBuild >= 48027)
                    //        source.ReadInt16();
                    //    else
                    //        source.ReadInt32();

                    //    // each byte has a max value of 0x7F (127)
                    //    source.stream.Position = source.stream.Position + (collectionSize * 2);
                    //}
                }

                source.ReadBoolean(); // m_hasSilencePenalty

                if (replay.ReplayBuild >= 61718)
                {
                    source.ReadBoolean();
                    source.ReadBoolean(); // m_hasVoiceSilencePenalty
                }

                if (replay.ReplayBuild >= 66977)
                    source.ReadBoolean(); // m_isBlizzardStaff

                if (source.ReadBoolean()) // is player in party
                    replay.ClientListByUserID[i].PartyValue = source.ReadLongBits(64); // players in same party will have the same exact 8 bytes of data

                source.ReadBoolean(); // has battle tag?
                replay.ClientListByUserID[i].BattleTag = source.ReadBlobAsString(7);

                if (!replay.ClientListByUserID[i].BattleTag.Contains('#'))
                    throw new StormParseException($"{ExceptionHeader}: Invalid battletag");

                if (replay.ReplayBuild >= 52860 || (replay.ReplayVersion.Major == 2 && replay.ReplayBuild >= 51978))
                    replay.ClientListByUserID[i].AccountLevel = (int)source.ReadBits(32);  // in custom games, this is a 0

                if (replay.ReplayBuild >= 69947)
                {
                    source.ReadBoolean(); // m_hasActiveBoost
                    source.ReadBits(2);
                }
                else
                {
                    source.ReadBits(3);
                }
            }
        }
    }
}
