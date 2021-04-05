using System;

namespace Denhub.Chat.Collector.Models {
    public record UnprocessedChatMessage {
        public string RawChatMessage { get; set; }
        public DateTime TimeReceived { get; set; }
    }
}