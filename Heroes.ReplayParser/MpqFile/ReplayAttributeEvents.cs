using Heroes.MpqTool;
using Heroes.ReplayParser.Decoders;
using Heroes.ReplayParser.Player;
using Heroes.ReplayParser.Replay;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Heroes.ReplayParser.MpqFile
{
    internal class ReplayAttributeEvents : IMpqFile
    {
        public string FileName { get; } = "replay.attributes.events";

        public void Parse(StormReplay replay, MpqBuffer mpqBuffer)
        {
            BitPackedBuffer bitPackedBuffer = new BitPackedBuffer(mpqBuffer, EndianType.LittleEndian);

            bitPackedBuffer.ReadBits(8); // source
            bitPackedBuffer.ReadBits(32); // mapNamespace
            uint count = bitPackedBuffer.ReadBits(32);

            List<ReplayAttribute> attributes = new List<ReplayAttribute>((int)count);

            for (int i = 0; i < count; i++)
            {
                bitPackedBuffer.ReadBits(32); // namespace

                attributes.Add(new ReplayAttribute()
                {
                    AttributeType = (ReplayAttributeEventType)bitPackedBuffer.ReadBits(32), // attrid
                    PlayerId = (int)BinaryPrimitives.ReverseEndianness(bitPackedBuffer.ReadBits(8)), // scope
                    Value = bitPackedBuffer.ReadBytes(4),
                });
            }

            // The 'PlayerID' in attributes does not seem to match any existing player array
            // It almost matches the 'Replay.Player' array, except for games with less than 10 players
            int replayPlayersWithOpenSlotsIndex = 1;

            Span<char> attributeValueBuffer = stackalloc char[4]; // used to hold the attribute value
            IOrderedEnumerable<ReplayAttribute> orderedAttributes = attributes.OrderBy(x => x.AttributeType);

            foreach (ReplayAttribute attribute in orderedAttributes)
            {
                Encoding.UTF8.GetChars(attribute.Value.Span, attributeValueBuffer);

                Span<char> attributeValue = attributeValueBuffer.Trim('\0');
                attributeValue.Reverse();

                switch (attribute.AttributeType)
                {
                    case ReplayAttributeEventType.PlayerTypeAttribute:
                        {
                            Span<char> value = stackalloc char[attributeValue.Length];
                            MemoryExtensions.ToLower(attributeValue, value, CultureInfo.InvariantCulture);

                            if (value.SequenceEqual("comp") || value.SequenceEqual("humn"))
                            {
                                replay.PlayersWithOpenSlots[attribute.PlayerId - 1] = replay.Players[attribute.PlayerId - replayPlayersWithOpenSlotsIndex];
                            }

                            if (value.SequenceEqual("comp"))
                                replay.PlayersWithOpenSlots[attribute.PlayerId - 1].PlayerType = PlayerType.Computer;
                            else if (value.SequenceEqual("humn"))
                                replay.PlayersWithOpenSlots[attribute.PlayerId - 1].PlayerType = PlayerType.Human;
                            else if (value.SequenceEqual("open"))
                                replayPlayersWithOpenSlotsIndex++; // Less than 10 players in a Custom game
                            else
                                throw new StormParseException($"Unexpected value for PlayerTypeAttribute: {value.ToString()}");

                            break;
                        }

                    case ReplayAttributeEventType.TeamSizeAttribute:
                        {
                            replay.TeamSize = attributeValue.ToString();
                            break;
                        }

                    case ReplayAttributeEventType.DifficultyLevelAttribute:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerDifficulty = attributeValue switch
                                {
                                    Span<char> _ when attributeValue.SequenceEqual("VyEy") => PlayerDifficulty.Beginner,
                                    Span<char> _ when attributeValue.SequenceEqual("Easy") => PlayerDifficulty.Recruit,
                                    Span<char> _ when attributeValue.SequenceEqual("Medi") => PlayerDifficulty.Adept,
                                    Span<char> _ when attributeValue.SequenceEqual("HdVH") => PlayerDifficulty.Veteran,
                                    Span<char> _ when attributeValue.SequenceEqual("VyHd") => PlayerDifficulty.Elite,
                                    _ => PlayerDifficulty.Unknown,
                                };
                            }

                            break;
                        }

                    case ReplayAttributeEventType.GameSpeedAttribute:
                        {
                            Span<char> value = stackalloc char[attributeValue.Length];
                            MemoryExtensions.ToLower(attributeValue, value, CultureInfo.InvariantCulture);

                            replay.GameSpeed = value switch
                            {
                                Span<char> _ when value.SequenceEqual("slor") => GameSpeed.Slower,
                                Span<char> _ when value.SequenceEqual("slow") => GameSpeed.Slow,
                                Span<char> _ when value.SequenceEqual("norm") => GameSpeed.Normal,
                                Span<char> _ when value.SequenceEqual("fast") => GameSpeed.Fast,
                                Span<char> _ when value.SequenceEqual("fasr") => GameSpeed.Faster,
                                _ => GameSpeed.Unknown,
                            };

                            break;
                        }

                    case ReplayAttributeEventType.GameModeAttribute:
                        {
                            Span<char> value = stackalloc char[attributeValue.Length];
                            MemoryExtensions.ToLower(attributeValue, value, CultureInfo.InvariantCulture);

                            switch (attributeValue)
                            {
                                case Span<char> _ when value.SequenceEqual("priv"):
                                    replay.GameMode = GameMode.Custom;
                                    break;
                                case Span<char> _ when value.SequenceEqual("amm"):
                                    if (replay.ReplayBuild < 33684)
                                        replay.GameMode = GameMode.QuickMatch;
                                    break;
                                default:
                                    throw new StormParseException($"Unexpected GameTypeAttribute: {value.ToString()}");
                            }

                            break;
                        }

                    case ReplayAttributeEventType.HeroAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.IsAutoSelect = attributeValue.SequenceEqual("Rand");
                                player.PlayerHero.HeroAttributeId = attributeValue.ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.SkinAndSkinTintAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.IsAutoSelect = attributeValue.SequenceEqual("Rand");
                                player.PlayerLoadout.SkinAndSkinTintAttributeId = attributeValue.ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.MountAndMountTintAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.MountAndMountTintAttributeId = attributeValue.ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.BannerAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.BannerAttributeId = attributeValue.ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.SprayAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.SprayAttributeId = attributeValue.ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.VoiceLineAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.VoiceLineAttributeId = attributeValue.ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.AnnouncerAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.AnnouncerPackAttributeId = attributeValue.ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.HeroLevel:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerHero.HeroLevel = int.Parse(attributeValue);

                                if (player.IsAutoSelect && player.PlayerHero.HeroLevel > 1)
                                    player.IsAutoSelect = false;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.LobbyMode:
                        {
                            if (replay.ReplayBuild < 43905 && replay.GameMode != GameMode.Custom)
                            {
                                Span<char> value = stackalloc char[attributeValue.Length];
                                MemoryExtensions.ToLower(attributeValue, value, CultureInfo.InvariantCulture);

                                switch (value)
                                {
                                    case Span<char> _ when value.SequenceEqual("stan"):
                                        replay.GameMode = GameMode.QuickMatch;
                                        break;
                                    case Span<char> _ when value.SequenceEqual("drft"):
                                        replay.GameMode = GameMode.HeroLeague;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            break;
                        }

                    case ReplayAttributeEventType.ReadyMode:
                        {
                            Span<char> value = stackalloc char[attributeValue.Length];
                            MemoryExtensions.ToLower(attributeValue, value, CultureInfo.InvariantCulture);

                            if (replay.ReplayBuild < 43905 && replay.GameMode == GameMode.HeroLeague && value.SequenceEqual("fcfs"))
                                replay.GameMode = GameMode.TeamLeague;
                            break;
                        }

                    case ReplayAttributeEventType.DraftTeam1Ban1:
                    case ReplayAttributeEventType.DraftTeam1Ban2:
                    case ReplayAttributeEventType.DraftTeam1Ban3:
                        {
                            if (replay.TeamHeroAttributeIdBans.TryGetValue(0, out List<string>? values))
                                values.Add(attributeValue.ToString());
                            else
                                replay.TeamHeroAttributeIdBans.Add(0, new List<string>() { attributeValue.ToString() });

                            break;
                        }

                    case ReplayAttributeEventType.DraftTeam2Ban1:
                    case ReplayAttributeEventType.DraftTeam2Ban2:
                    case ReplayAttributeEventType.DraftTeam2Ban3:
                        {
                            if (replay.TeamHeroAttributeIdBans.TryGetValue(1, out List<string>? values))
                                values.Add(attributeValue.ToString());
                            else
                                replay.TeamHeroAttributeIdBans.Add(1, new List<string>() { attributeValue.ToString() });

                            break;
                        }
                }
            }
        }
    }
}
