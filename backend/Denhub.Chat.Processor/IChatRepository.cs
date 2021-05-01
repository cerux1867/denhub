using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;

namespace Denhub.Chat.Processor {
    public interface IChatMessageRepository {
        public Task AddAsync(TwitchChatMessage message);
    }
}