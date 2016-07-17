using System;
using static Heroes.ReplayParser.ReplayMessageEvents;

namespace Heroes.ReplayParser
{
    public class Message
    {
        public MessageEventType MessageEventType { get; set; }
        public MessageBase MessageDisplayed { get; set; }
        public TimeSpan Timestamp { get { return MessageDisplayed.Timestamp; } }

        public override string ToString()
        {
            return MessageDisplayed.ToString();
        }
    }
}
