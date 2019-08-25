using System;
using System.Text;

namespace Heroes.ReplayParser.MpqFile
{
    internal class ReplayAttribute
    {
        public int Namespace { get; set; }
        public ReplayAttributeEventType AttributeType { get; set; }
        public int PlayerId { get; set; }
        public ReadOnlyMemory<byte> Value { get; set; }

        public override string? ToString()
        {
            return $"{nameof(PlayerId)}: {PlayerId}, {nameof(AttributeType)}: {AttributeType}, {nameof(Value)}: {Encoding.UTF8.GetString(Value.Span)}";
        }
    }
}
