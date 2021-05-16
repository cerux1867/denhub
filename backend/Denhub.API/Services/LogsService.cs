using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Repositories;
using Denhub.API.Results;
using Denhub.Common.Models;

namespace Denhub.API.Services {
    public class LogsService : ILogsService {
        private readonly IChatLogsRepository _chatLogsRepository;
        private readonly ITwitchClient _twitchClient;

        public LogsService(IChatLogsRepository repo, ITwitchClient twitchClient) {
            _chatLogsRepository = repo;
            _twitchClient = twitchClient;
        }

        public async Task<ValueResult<TwitchChatMessagesResult>> GetByChannelAsync(long channelId, DateTime timestamp,
            string order, string paginationCursor = null, int limit = 100) {
            var messagesResult = await GetByChannelIdAsync(channelId, timestamp, order, paginationCursor, limit);
            return Result.Ok(messagesResult);
        }

        public async Task<ValueResult<TwitchChatMessagesResult>> GetByChannelAsync(string channelName,
            DateTime timestamp,
            string order, string paginationCursor = null, int limit = 100) {
            var channelResponse = await _twitchClient.GetUsersAsync(new List<string> {
                channelName
            });
            if (channelResponse.Data.Count() != 1) {
                return Result.NotFound<TwitchChatMessagesResult>("Ambiguous channel name");
            }

            var channelId = channelResponse.Data.First().Id;
            var messagesResult =
                await GetByChannelIdAsync(Convert.ToInt64(channelId), timestamp, order, paginationCursor, limit);
            return Result.Ok(messagesResult);
        }

        private async Task<TwitchChatMessagesResult> GetByChannelIdAsync(long channelId, DateTime timestamp,
            string order, string paginationCursor, int limit) {
            var modifiedTimestamp = timestamp.Year < 1970 ? DateTime.UnixEpoch : timestamp;
            var (cursor, chatMessages) =
                await _chatLogsRepository.GetByChannelIdAsync(channelId, order == "asc", modifiedTimestamp,
                    paginationCursor, limit);
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