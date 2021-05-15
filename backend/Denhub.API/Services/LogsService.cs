using System;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Repositories;
using Denhub.Common.Models;

namespace Denhub.API.Services {
    public class LogsService : ILogsService {
        private readonly IChatLogsRepository _chatLogsRepository;

        public LogsService(IChatLogsRepository repo) {
            _chatLogsRepository = repo;
        }

        public async Task<TwitchChatMessagesResult> GetByChannelIdAsync(long channelId, DateTime timestamp,
            string paginationCursor = null, int limit = 100) {
            var modifiedTimestamp = timestamp.Year < 1970 ? DateTime.UnixEpoch : timestamp; 
            var (cursor, chatMessages) = await _chatLogsRepository.GetByChannelIdAsync(channelId, modifiedTimestamp, paginationCursor, limit);
            var modifiedChatMessages = chatMessages.Select(msg => new TwitchChatMessagePublic {
                    Badges = msg.Badges,
                    Emotes = msg.Emotes,
                    Message = msg.Message,
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(msg.Timestamp).UtcDateTime,
                    ChannelId = msg.ChannelId,
                    MessageId = msg.MessageId,
                    UserColor = msg.UserColor,
                    UserId = msg.UserId,
                    ChannelDisplayName = msg.ChannelDisplayName,
                    UserDisplayName = msg.UserDisplayName
                })
                .ToList();

            return new TwitchChatMessagesResult {
                ChatMessages = modifiedChatMessages,
                PaginationCursor = cursor
            };
        }
    }
}