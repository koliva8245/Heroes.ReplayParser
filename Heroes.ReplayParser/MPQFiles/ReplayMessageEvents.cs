﻿namespace Heroes.ReplayParser
{
    using System;
    using System.IO;
    using System.Text;

    public class ReplayMessageEvents
    {
        public const string FileName = "replay.message.events";

        /// <summary> Parses the Replay.Messages.Events file. </summary>
        /// <param name="buffer"> Buffer containing the contents of the replay.messages.events file. </param>
        /// <returns> A list of messages parsed from the buffer. </returns>
        public static void Parse(Replay replay, byte[] buffer, Player[] clientList)
        {
            var ticksElapsed = 0;
            using (var stream = new MemoryStream(buffer))
            {
                var bitReader = new Streams.BitReader(stream);

                while (!bitReader.EndOfStream)
                {
                    var message = new Message();

                    ticksElapsed += (int)bitReader.Read(6 + (bitReader.Read(2) << 3));
                    int playerIndex = (int)bitReader.Read(5);

                    message.MessageEventType = (MessageEventType)bitReader.Read(4);
                    switch (message.MessageEventType)
                    {
                        case MessageEventType.SChatMessage:
                            {
                                ChatMessage chatMessage = new ChatMessage();

                                chatMessage.MessageTarget = (MessageTarget)bitReader.Read(3); // m_recipient (the target)
                                chatMessage.Message = Encoding.UTF8.GetString(bitReader.ReadBlobPrecededWithLength(11)); // m_string

                                chatMessage.Timestamp = new TimeSpan(0, 0, (int)Math.Round(ticksElapsed / 16.0));
                                chatMessage.PlayerId = playerIndex;
                                chatMessage.PlayerName = clientList[playerIndex].Name;
                                chatMessage.CharacterName = clientList[playerIndex].Character;

                                message.MessageDisplayed = chatMessage;
                                replay.Messages.Add(message);
                                break;
                            }
                        case MessageEventType.SPingMessage:
                            {
                                PingMessage pingMessage = new PingMessage();
                                    
                                pingMessage.MessageTarget = (MessageTarget)bitReader.Read(3); // m_recipient (the target) 

                                pingMessage.XCoordinate = bitReader.ReadInt32() - (-2147483648); // m_point x
                                pingMessage.YCoordinate = bitReader.ReadInt32() - (-2147483648); // m_point y      

                                pingMessage.Timestamp = new TimeSpan(0, 0, (int)Math.Round(ticksElapsed / 16.0));
                                pingMessage.PlayerId = playerIndex;

                                message.MessageDisplayed = pingMessage;
                                replay.Messages.Add(message);
                                break;
                            }
                        case MessageEventType.SLoadingProgressMessage:
                            {
                                // can be used to keep track of how fast/slow players are loading
                                // also includes players who are reconnecting
                                var progress = bitReader.ReadInt32() - (-2147483648); // m_progress
                                break;
                            }
                        case MessageEventType.SServerPingMessage:
                            {
                                break;
                            }
                        case MessageEventType.SReconnectNotifyMessage:
                            {
                                bitReader.Read(2); // m_status; is either a 1 or a 2
                                break;
                            }
                        case MessageEventType.SPlayerAnnounceMessage:
                            {
                                PlayerAnnounceMessage announceMessage = new PlayerAnnounceMessage();

                                announceMessage.AnnouncementType = (AnnouncementType)bitReader.Read(2);

                                switch (announceMessage.AnnouncementType)
                                {
                                    case AnnouncementType.None:
                                        {
                                            break;
                                        }
                                    case AnnouncementType.Ability:
                                        {
                                            AbilityAnnouncment ability = new AbilityAnnouncment();
                                            ability.AbilityLink = bitReader.ReadInt16(); // m_abilLink      
                                            ability.AbilityIndex = (int)bitReader.Read(5); // m_abilCmdIndex
                                            ability.ButtonLink = bitReader.ReadInt16(); // m_buttonLink

                                            bitReader.ReadInt32(); // m_otherUnitTag
                                            bitReader.ReadInt32(); // m_unitTag

                                            announceMessage.AnnouncementDisplayed = ability;
                                            break;
                                        }

                                    case AnnouncementType.Behavior: // no idea what triggers this
                                        {
                                            bitReader.ReadInt16(); // m_behaviorLink
                                            bitReader.ReadInt16(); // m_buttonLink

                                            bitReader.ReadInt32(); // m_otherUnitTag
                                            bitReader.ReadInt32(); // m_unitTag
                                            break;
                                        }
                                    case AnnouncementType.Vitals:
                                        {
                                            VitalAnnouncment vital = new VitalAnnouncment();
                                            vital.VitalType = (VitalType)(bitReader.ReadInt16() - (-32768));

                                            bitReader.ReadInt32(); // m_otherUnitTag
                                            bitReader.ReadInt32(); // m_unitTag

                                            announceMessage.AnnouncementDisplayed = vital;
                                            break;
                                        }
                                    default:
                                        throw new NotImplementedException();
                                }

                                announceMessage.Timestamp = new TimeSpan(0, 0, (int)Math.Round(ticksElapsed / 16.0));
                                announceMessage.PlayerId = playerIndex;
                                message.MessageDisplayed = announceMessage;
                                    

                                replay.Messages.Add(message);
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }

                    bitReader.AlignToByte();
                }
            }
        }

        public enum MessageEventType
        {
            SChatMessage = 0,
            SPingMessage = 1,
            SLoadingProgressMessage = 2,
            SServerPingMessage = 3,
            SReconnectNotifyMessage = 4,
            SPlayerAnnounceMessage = 5
        }

        // m_announcement
        public enum AnnouncementType
        {
            None = 0,
            Ability = 1,
            Behavior = 2,
            Vitals = 3
        }

        public enum MessageTarget
        {
            All = 0,
            Allies = 1,
            //Observers = ?,

            // party? private?
        }

        public enum VitalType
        {
            Health = 0,
            Mana = 2 // also includes Fury and Brew
        }

        public class AbilityAnnouncment : AnnouncementBase
        {
            public int AbilityIndex { get; set; } 
            public int AbilityLink { get; set; }
            public int ButtonLink { get; set; }
        }

        public class BehaviorAnnouncment : AnnouncementBase
        { }

        public class VitalAnnouncment : AnnouncementBase
        {
            public VitalType VitalType { get; set; }
        }

        public class AnnouncementBase
        { }

        public class ChatMessage : MessageBase
        {
            public MessageTarget MessageTarget { get; set; }
            public string PlayerName { get; set; }
            public string CharacterName { get; set; }
            public string Message { get; set; }
            public override string ToString()
            {
                if (!string.IsNullOrEmpty(CharacterName))
                    return $"({Timestamp}) [{MessageTarget}] {PlayerName} ({CharacterName}): {Message}";
                else
                    return $"({Timestamp}) [{MessageTarget}] {PlayerName}: {Message}";

            }
        }

        // Ping messages include normal pings (no target), targeted pings (such as Player 1 wants to help Player 2), retreat,
        // and the more ping options (on my way, defend, danger, assist)
        // does not include captured camps, hearthing
        // no way to tell which one is which
        public class PingMessage : MessageBase
        {
            public MessageTarget MessageTarget { get; set; }
            public int XCoordinate { get; set; }
            public int YCoordinate { get; set; }

            public override string ToString()
            {
                return $"({Timestamp}) [{MessageTarget}] Player {PlayerId} used a ping";
            }
        }

        public class PlayerAnnounceMessage : MessageBase
        {
            public AnnouncementBase AnnouncementDisplayed { get; set; }
            public AnnouncementType AnnouncementType { get; set; }

            public override string ToString()
            {
                return $"({Timestamp}) Player {PlayerId} announced {AnnouncementType}";
            }
        }

        public class MessageBase
        {
            public int PlayerId { get; set; }
            public TimeSpan Timestamp { get; set; }
        }
    }
}