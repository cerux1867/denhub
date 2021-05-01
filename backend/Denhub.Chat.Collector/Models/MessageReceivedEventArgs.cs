using System;
using Denhub.Common;

namespace Denhub.Chat.Collector.Models {
    public class MessageReceivedEventArgs : EventArgs {
        public UnprocessedChatMessage UnprocessedMessage { get; set; }
    }
}