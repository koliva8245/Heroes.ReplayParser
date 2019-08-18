using System;
using System.Runtime.Serialization;

namespace Heroes.ReplayParser
{
    [Serializable]
    public class StormParseException : Exception
    {
        public StormParseException()
        {
        }

        public StormParseException(string? message)
            : base(message)
        {
        }

        public StormParseException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected StormParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
