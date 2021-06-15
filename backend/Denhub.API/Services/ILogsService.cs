using System;
using System.Threading.Tasks;
using Denhub.API.Results;
using MongoDB.Driver;

namespace Denhub.API.Services {
    public interface ILogsService {
        /// <summary>
        /// Retrieves a paged set of chat messages from a channel
        /// </summary>
        /// <param name="channelId">Twitch channel ID</param>
        /// <param name="startDate">An optional timestamp value from which on to match results. Default is <see cref="DateTime.UnixEpoch"/></param>
        /// <param name="endDate">An optional timestamp value to which to match results.</param>
        /// <param name="order">Defines sorting order. Default is <see cref="SortDirection.Descending"/></param>
        /// <param name="paginationCursor">An optional pagination cursor from which to continue result matching.</param>
        /// <param name="limit">An optional integer that defines the size of the page in pagination. Default is 100.</param>
        /// <returns>A <see cref="ValueResult{T}"/> which includes a <see cref="PagedResult"/> if successful</returns>
        public Task<ValueResult<PagedResult>> GetByChannelIdAsync(long channelId, DateTime? startDate = null, DateTime? endDate = null, SortDirection order = SortDirection.Descending, string paginationCursor = null, int limit = 100);

        /// <summary>
        /// Retrieves a paged set of chat messages from a channel
        /// </summary>
        /// <param name="channelName">Twitch channel name</param>
        /// <param name="startDate">An optional timestamp value from which on to match results. Default is <see cref="DateTime.UnixEpoch"/></param>
        /// <param name="endDate">An optional timestamp value to which to match results.</param>
        /// <param name="order">Defines sorting order. Default is <see cref="SortDirection.Descending"/></param>
        /// <param name="paginationCursor">An optional pagination cursor from which to continue result matching.</param>
        /// <param name="limit">An optional integer that defines the size of the page in pagination. Default is 100.</param>
        /// <returns>A <see cref="ValueResult{T}"/> which includes a <see cref="PagedResult"/> if successful</returns>
        public Task<ValueResult<PagedResult>> GetByChannelNameAsync(string channelName, DateTime? startDate = null,
            DateTime? endDate = null, SortDirection order = SortDirection.Descending, string paginationCursor = null,
            int limit = 100);
    }
}