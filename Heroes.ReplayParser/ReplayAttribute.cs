namespace Heroes.ReplayParser
{
    internal struct ReplayAttribute
    {
        public ReplayAttributeEventType AttributeType;
        public int PlayerId;
        public string Value;

        public override string? ToString()
        {
            return $"{nameof(PlayerId)}: {PlayerId}, {nameof(AttributeType)}: {AttributeType}, {nameof(Value)}: {Value}";
        }
    }
}
