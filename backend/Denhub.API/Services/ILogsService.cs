using System;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Results;

namespace Denhub.API.Services {
    public interface ILogsService {
        public Task<ValueResult<TwitchChatMessagesResult>> GetByChannelAsync(long channelId, DateTime timestamp,
            string paginationCursor = null, int limit = 100);
        public Task<ValueResult<TwitchChatMessagesResult>> GetByChannelAsync(string channelName, DateTime timestamp,
            string paginationCursor = null, int limit = 100);
    }
}