using System;

namespace Denhub.Chat.Collector.Models {
    public class MessageReceivedEventArgs : EventArgs {
        public UnprocessedChatMessage UnprocessedMessage { get; set; }
    }
}