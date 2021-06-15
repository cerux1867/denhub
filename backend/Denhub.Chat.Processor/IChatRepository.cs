using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;
using Denhub.Common.Models;

namespace Denhub.Chat.Processor {
    public interface IChatMessageRepository {
        public Task AddAsync(TwitchChatMessage messageBackend);
    }
}