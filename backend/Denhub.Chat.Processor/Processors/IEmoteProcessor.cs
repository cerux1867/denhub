using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;

namespace Denhub.Chat.Processor.Processors {
    public interface IEmoteProcessor {
        public Task<TwitchChatMessage> EnrichWithExternalEmotesAsync(TwitchChatMessage chatMessage);
    }
}