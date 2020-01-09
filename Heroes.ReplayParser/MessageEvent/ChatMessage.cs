namespace Heroes.ReplayParser.MessageEvent
{
    /// <summary>
    /// Contains the information for a chat message.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Gets or sets the target of the message.
        /// </summary>
        public StormMessageTarget MessageTarget { get; internal set; }

        /// <summary>
        /// Gets or sets the message sent.
        /// </summary>
        public string? Message { get; internal set; } = null;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{MessageTarget}] {Message}";
        }
    }
}
