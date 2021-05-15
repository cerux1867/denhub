using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;
using Denhub.Common.Models;

namespace Denhub.Chat.Processor.Processors {
    public interface IEmoteProcessor {
        public Task<TwitchChatMessageBackend> EnrichWithExternalEmotesAsync(TwitchChatMessageBackend chatMessageBackend);
    }
}