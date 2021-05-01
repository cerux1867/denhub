using System;

namespace Denhub.Common {
    public record UnprocessedChatMessage {
        public string RawChatMessage { get; set; }
        public DateTime TimeReceived { get; set; }
    }
}