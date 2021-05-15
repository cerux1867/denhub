using Denhub.Chat.Processor.Models;
using Denhub.Common.Models;

namespace Denhub.Chat.Processor.Processors {
    public interface IChatMessagePreprocessor {
        public TwitchChatMessageBackend ProcessMessage(string message);
    }
}