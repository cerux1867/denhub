using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;
using Denhub.Common;
using Denhub.Common.Models;
using Microsoft.Extensions.Logging;

namespace Denhub.Chat.Processor.Caches {
    public class InMemoryBadgeCache : IBadgeCache {
        private readonly ILogger<InMemoryBadgeCache> _logger;
        private readonly Dictionary<long, List<TwitchBadge>> _channelBadgeDict;
        private readonly List<TwitchBadge> _globalBadges;
        private readonly HttpClient _httpClient;
        private readonly Uri _badgeUri = new("https://badges.twitch.tv/v1/badges/");

        private record RawVersionedBadge(string image_url_1x, string image_url_2x, string image_url_4x, string ClickUrl, string Title, string Description);
        
        public InMemoryBadgeCache(ILogger<InMemoryBadgeCache> logger, HttpClient client) {
            _logger = logger;
            _httpClient = client;
            _httpClient.BaseAddress = _badgeUri;
            _globalBadges = new List<TwitchBadge>();
            _channelBadgeDict = new Dictionary<long, List<TwitchBadge>>();
        }
        
        public async Task<List<TwitchBadge>> GetChannelCachedAsync(long channelId) {
            var badgeList = new List<TwitchBadge>();
            
            if (_globalBadges.Count == 0) {
                var globalBadgeList = await DownloadAsync("global/display");

                _globalBadges.AddRange(globalBadgeList);
                _logger.LogInformation("Cached {NumGlobalBadges} global badges", globalBadgeList.Count);
            }
            
            badgeList.AddRange(_globalBadges);
            
            if (!_channelBadgeDict.ContainsKey(channelId)) {
                // channel badges are not cached yet, download and cache them here
                var channelBadgeList = await DownloadAsync($"channels/{channelId}/display");
                
                _channelBadgeDict.Add(channelId, channelBadgeList);
                _logger.LogInformation("Cached {ChannelBadgeCount} channel badges for {ChannelId}", channelBadgeList.Count, channelId);
            }

            _channelBadgeDict.TryGetValue(channelId, out var channelBadges);
            badgeList.AddRange(channelBadges);

            return badgeList;
        }

        private async Task<List<TwitchBadge>> DownloadAsync(string url) {
            var badgeList = new List<TwitchBadge>();
            // global badges are not cached yet, download and cache them here
            var badgesResponse = await _httpClient.GetAsync(url);
            var badgesRawBody = await badgesResponse.Content.ReadAsStringAsync();
            var badgesJsonBody = JsonSerializer.Deserialize<ExpandoObject>(badgesRawBody, new JsonSerializerOptions {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            });
            var badgeSet =
                JsonSerializer.Deserialize<ExpandoObject>(badgesJsonBody.First(v => v.Key == "badge_sets").Value
                    .ToString());
            foreach (var (s, o) in badgeSet) {
                var rawBadge = JsonSerializer.Deserialize<ExpandoObject>(o.ToString()).FirstOrDefault(v => v.Key == "versions")
                    .Value;
                var rawVersions = JsonSerializer.Deserialize<ExpandoObject>(rawBadge.ToString());
                foreach (var (key, value) in rawVersions) {
                    var rawVersionedBadge = JsonSerializer.Deserialize<RawVersionedBadge>(value.ToString(),
                        new JsonSerializerOptions {
                            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
                        });
                    var parsedBadge = new TwitchBadge {
                        Name = s,
                        Version = key,
                        Title = rawVersionedBadge.Title,
                        Description = rawVersionedBadge.Description,
                        Type = s switch {
                            "subscriber" => BadgeType.Subscriber,
                            "bits" => BadgeType.Bits,
                            "sub-gifter" => BadgeType.SubGifter,
                            _ => BadgeType.Other
                        },
                        ImageUrls = new List<string> {
                            rawVersionedBadge.image_url_1x,
                            rawVersionedBadge.image_url_2x,
                            rawVersionedBadge.image_url_4x
                        }
                    };
                    badgeList.Add(parsedBadge);
                }
            }

            return badgeList;
        }
    }
}