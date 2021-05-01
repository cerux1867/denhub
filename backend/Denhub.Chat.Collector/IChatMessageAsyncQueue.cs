using System.Threading.Tasks;
using Denhub.Common;

namespace Denhub.Chat.Collector {
    public interface IChatMessageAsyncQueue {
        public Task EnqueueAsync(UnprocessedChatMessage message);
    }
}