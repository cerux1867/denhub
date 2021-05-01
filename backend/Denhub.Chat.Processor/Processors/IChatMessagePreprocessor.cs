using Denhub.Chat.Processor.Models;

namespace Denhub.Chat.Processor.Processors {
    public interface IChatMessagePreprocessor {
        public TwitchChatMessage ProcessMessage(string message);
    }
}