﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Models.Twitch;
using StackExchange.Redis;

namespace Denhub.API.Services {
    /// <summary>
    /// Repository pattern over Redis and Twitch API that implements a read-through caching strategy.
    /// </summary>
    public class RedisVodRepository : IVodRepository {
        private readonly ITwitchClient _twitchClient;
        private readonly IConnectionMultiplexer _multiplexer;

        public RedisVodRepository(ITwitchClient twitchClient, IConnectionMultiplexer multiplexer) {
            _twitchClient = twitchClient;
            _multiplexer = multiplexer;
        }

        public async Task<IEnumerable<CommonVodModel>> GetOrFetchVodsAsync(int channelId, int offset = 0,
            int limit = 100) {
            var cacheExists = await _multiplexer.GetDatabase().KeyExistsAsync(channelId.ToString());
            if (!cacheExists) {
                await RefreshCacheAsync(channelId);
            }
            else {
                var refreshCheckItem =
                    await _twitchClient.GetVideosByUserIdAsync(channelId, null, 1, TwitchVideoItemType.Archive);
                if (refreshCheckItem.Data.Any()) {
                    var item = refreshCheckItem.Data.First();
                    var refreshCacheItem = JsonSerializer.Deserialize<CommonVodModel>(await _multiplexer.GetDatabase().ListGetByIndexAsync(channelId.ToString(), 0));
                    if (item.PublishedAt != refreshCacheItem?.Date) {
                        await RefreshCacheAsync(channelId);
                    }
                }
            }

            var cachedVods = await _multiplexer.GetDatabase().ListRangeAsync(channelId.ToString(), offset, limit);
            return cachedVods.Select(vod => JsonSerializer.Deserialize<CommonVodModel>(vod)).Where(vodModel => !string.IsNullOrEmpty(vodModel?.ThumbnailUrl));
        }

        private async Task RefreshCacheAsync(int channelId) {
            var sequencedResponse =
                await _twitchClient.GetVideosByUserIdAsync(channelId, null, 100, TwitchVideoItemType.Archive);
            var vodList = sequencedResponse.Data.Select(videoItem =>
                    new CommonVodModel {
                        PlatformVodId = Convert.ToInt32(videoItem.Id),
                        Date = videoItem.PublishedAt,
                        Length = Convert.ToInt32(videoItem.Duration.TotalSeconds),
                        Title = videoItem.Title,
                        Type = VodType.Twitch,
                        ThumbnailUrl = videoItem.ThumbnailUrl
                    })
                .ToList();
            while (!string.IsNullOrEmpty(sequencedResponse.Pagination.Cursor)) {
                sequencedResponse = await _twitchClient.GetVideosByUserIdAsync(channelId,
                    sequencedResponse.Pagination.Cursor, 100, TwitchVideoItemType.Archive);
                if (sequencedResponse.Data.Any()) {
                    vodList.AddRange(sequencedResponse.Data.Select(
                        videoItem => new CommonVodModel {
                            PlatformVodId = Convert.ToInt32(videoItem.Id),
                            Date = videoItem.PublishedAt,
                            Length = Convert.ToInt32(videoItem.Duration.TotalSeconds),
                            Title = videoItem.Title,
                            Type = VodType.Twitch,
                            ThumbnailUrl = videoItem.ThumbnailUrl
                        }));
                }
            }

            var redisValues = vodList.Select(vod => new RedisValue(JsonSerializer.Serialize(vod)));
            await _multiplexer.GetDatabase().ListRightPushAsync(channelId.ToString(), redisValues.ToArray());
            await _multiplexer.GetDatabase().KeyExpireAsync(channelId.ToString(), TimeSpan.FromMinutes(10));
        }
    }
}