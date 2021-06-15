using System;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Repositories;
using Denhub.API.Results;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Denhub.API.Services {
    public class MongoChatLogsService : ILogsService {
        private readonly IChatLogsRepository _chatLogsRepo;
        private readonly ITwitchClient _twitchClient;

        public MongoChatLogsService(IChatLogsRepository chatLogsRepo, ITwitchClient twitchClient) {
            _chatLogsRepo = chatLogsRepo;
            _twitchClient = twitchClient;
        }

        /// <inheritdoc />
        public async Task<ValueResult<PagedResult>> GetByChannelIdAsync(long channelId, DateTime? startDate = null,
            DateTime? endDate = null, SortDirection order = SortDirection.Descending, string paginationCursor = null,
            int limit = 100, string username = "") {
            var allowedChannels = await (await _chatLogsRepo.GetAllAsync())
                .Select(l => l.ChannelId).Distinct().ToListAsync();

            if (!allowedChannels.Contains(channelId)) {
                return Result.NotFound<PagedResult>("This channel has no logs stored or it does not exist");
            }

            var startAdjustedTimestamp = DateTimeOffset.UnixEpoch;
            if (startDate != null && startDate.Value >= DateTime.UnixEpoch) {
                startAdjustedTimestamp = startDate.Value;
            }
            var logs = await _chatLogsRepo.GetAllAsync();
            var queryableCollection = logs
                .Where(l => l.ChannelId == channelId && l.Timestamp >= startAdjustedTimestamp.ToUnixTimeMilliseconds());

            if (!string.IsNullOrEmpty(username)) {
                var queryUserResponse = await _twitchClient.GetUsersAsync(new[] {username});
                if (!queryUserResponse.Data.Any()) {
                    return Result.NotFound<PagedResult>("User does was not found");
                }

                var userId = Convert.ToInt64(queryUserResponse.Data.First().Id);
                queryableCollection = queryableCollection
                    .Where(l => l.UserId == userId);
            }
            
            if (endDate.HasValue) {
                queryableCollection = queryableCollection
                    .Where(l => l.Timestamp <= new DateTimeOffset(endDate.Value).ToUnixTimeMilliseconds());
            }
            var totalCount = await queryableCollection.CountAsync();
            if (!string.IsNullOrEmpty(paginationCursor)) {
                queryableCollection = queryableCollection
                    .Where(l => l.Id > ObjectId.Parse(paginationCursor));
            }

            queryableCollection = queryableCollection
                .Take(limit);
            if (order == SortDirection.Descending) {
                queryableCollection = queryableCollection
                    .OrderByDescending(l => l.Timestamp);
            } else if (order == SortDirection.Ascending) {
                queryableCollection = queryableCollection
                    .OrderBy(l => l.Timestamp);
            }
            var list = await queryableCollection.ToListAsync();
            return new SuccessValueResult<PagedResult>(new PagedResult {
                ChatMessages = list,
                Metadata = new PaginationMetadata {
                    PaginationCursor = list.Any() ? list.Last().Id.ToString() : string.Empty,
                    TotalCount = totalCount
                }
            });
        }

        public async Task<ValueResult<PagedResult>> GetByChannelNameAsync(string channelName, DateTime? startDate = null,
            DateTime? endDate = null, SortDirection order = SortDirection.Descending, string paginationCursor = null,
            int limit = 100, string username = "") {
            var channelQueryResponse = await _twitchClient.GetUsersAsync(new[] {channelName});
            if (!channelQueryResponse.Data.Any()) {
                return Result.NotFound<PagedResult>("This channel has no logs stored or it does not exist");
            }

            var channelId = Convert.ToInt64(channelQueryResponse.Data.First().Id);
            var result = await GetByChannelIdAsync(channelId, startDate, endDate, order, paginationCursor, limit, username);
            return result;
        }
    }
}