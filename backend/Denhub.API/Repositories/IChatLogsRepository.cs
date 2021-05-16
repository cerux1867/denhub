using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.Common.Models;

namespace Denhub.API.Repositories {
    public interface IChatLogsRepository {
        public Task<(string, IEnumerable<TwitchChatMessageBackend>)> GetByChannelIdAsync(long channelId, bool isAscending,
            DateTime timestamp, string paginationToken = null, int limit = 100);
    }
}