using System;
using System.Threading.Tasks;
using Denhub.API.Models;

namespace Denhub.API.Services {
    public interface ILogsService {
        public Task<TwitchChatMessagesResult> GetByChannelIdAsync(long channelId, DateTime timestamp, string paginationCursor = null, int limit = 100);
    }
}