using System.Threading.Tasks;
using Denhub.Chat.Collector.Models;

namespace Denhub.Chat.Collector {
    public interface IChatMessageAsyncQueue {
        public Task EnqueueAsync(UnprocessedChatMessage message);
    }
}