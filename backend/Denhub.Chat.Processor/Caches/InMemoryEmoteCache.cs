using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;
using Denhub.Common;
using Microsoft.Extensions.Logging;

namespace Denhub.Chat.Processor.Caches {
    public class InMemoryEmoteCache : IEmoteCache {
        private readonly ILogger<InMemoryEmoteCache> _logger;
        private readonly Dictionary<long, List<CachedEmote>> _emoteDict;
        private readonly HttpClient _httpClient;
        private readonly Uri _bttvApiBaseUri;
        private readonly Uri _bttvCdnBaseUri;
        private readonly Uri _ffzBaseUri;

        private record BttvEmote(string Id, string Code, string ImageType, string UserId);
        private record FfzEmote(int Id, string Name, JsonElement Urls);
        private record FfzSet(int Id, List<FfzEmote> Emoticons);

        public InMemoryEmoteCache(ILogger<InMemoryEmoteCache> logger, HttpClient client) {
            _logger = logger;
            _emoteDict = new Dictionary<long, List<CachedEmote>>();
            _httpClient = client;
            _bttvApiBaseUri = new Uri("https://api.betterttv.net/3/cached/");
            _bttvCdnBaseUri = new Uri("https://cdn.betterttv.net/emote");
            _ffzBaseUri = new Uri("https://api.frankerfacez.com/v1");
        }

        public async Task<List<CachedEmote>> GetChannelCachedAsync(long channelId) {
            return await GetOrDownloadEmotesAsync(channelId);
        }

        private async Task<List<CachedEmote>> GetOrDownloadEmotesAsync(long channelId) {
            var cachedEmotes = new List<CachedEmote>();

            if (_emoteDict.ContainsKey(0)) {
                _emoteDict.TryGetValue(0, out var cachedGlobalEmotes);
                if (cachedGlobalEmotes != null) {
                    cachedEmotes.AddRange(cachedGlobalEmotes);
                }
            }
            else {
                var bttvEmoteResponse = await _httpClient.GetAsync($"{_bttvApiBaseUri}emotes/global");
                if (!bttvEmoteResponse.IsSuccessStatusCode) {
                    throw new Exception("Unable to retrieve global BTTV emotes, emote enrichment will fail.");
                }

                var bttvGlobalEmoteList =
                    JsonSerializer.Deserialize<List<BttvEmote>>(
                        await bttvEmoteResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                var parsedGlobalEmoteList = bttvGlobalEmoteList.Select(unparsedEmote => new CachedEmote {
                    Name = unparsedEmote.Code, Urls = new List<Uri> {
                        new($"{_bttvCdnBaseUri}/{unparsedEmote.Id}/1x"),
                        new($"{_bttvCdnBaseUri}/{unparsedEmote.Id}/2x"),
                        new($"{_bttvCdnBaseUri}/{unparsedEmote.Id}/3x")
                    },
                    EmotePlatform = EmotePlatform.BetterTTV
                }).ToList();
                _emoteDict.Add(0, parsedGlobalEmoteList);
                _logger.LogInformation("Cached {NumberOfGlobalEmotes} emotes", parsedGlobalEmoteList.Count);
                cachedEmotes.AddRange(parsedGlobalEmoteList);
            }

            if (_emoteDict.ContainsKey(channelId)) {
                _emoteDict.TryGetValue(channelId, out var cachedChannelEmotes);
                if (cachedChannelEmotes != null) {
                    cachedEmotes.AddRange(cachedChannelEmotes);
                }
            }
            else {
                var bttvChannelEmotesResponse = await _httpClient.GetAsync($"{_bttvApiBaseUri}users/twitch/{channelId}");
                var tempBttvDynObject = JsonSerializer.Deserialize<ExpandoObject>(
                    await bttvChannelEmotesResponse.Content.ReadAsStringAsync());
                var bttvChannelEmotes =
                    JsonSerializer.Deserialize<List<BttvEmote>>(tempBttvDynObject.First(v => v.Key == "channelEmotes")
                        .Value?.ToString(), new JsonSerializerOptions {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                var bttvSharedEmotes = JsonSerializer.Deserialize<List<BttvEmote>>(tempBttvDynObject.First(v => v.Key == "sharedEmotes")
                    .Value?.ToString(), new JsonSerializerOptions {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var ffzChannelEmotesResponse = await _httpClient.GetAsync($"{_ffzBaseUri}/room/id/{channelId}");
                var ffzChannelEmotes = new List<CachedEmote>();
                
                if (ffzChannelEmotesResponse.IsSuccessStatusCode) {
                    var tempFfzDynObject = JsonSerializer.Deserialize<ExpandoObject>(
                        await ffzChannelEmotesResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions {
                            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
                        })!.First(v => v.Key == "sets").Value!.ToString();
                    var emoteSetsObject = JsonSerializer.Deserialize<ExpandoObject>(tempFfzDynObject);
                    
                    foreach (var keyValuePair in emoteSetsObject.AsEnumerable()) {
                        var emoteSet = JsonSerializer.Deserialize<FfzSet>(keyValuePair.Value.ToString(),
                            new JsonSerializerOptions {
                                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
                            });
                        foreach (var (_, name, jsonElement) in emoteSet.Emoticons) {
                            var urls = new List<Uri>();
                            var expandedUrls = JsonSerializer.Deserialize<ExpandoObject>(jsonElement.ToString());
                            foreach (var emotePair in expandedUrls) {
                                var url = emotePair.Value.ToString();
                                if (url.StartsWith("//")) {
                                    url = $"https:{url}";
                                }
                                urls.Add(new Uri(url));
                            }
                            ffzChannelEmotes.Add(new CachedEmote {
                                Name = name,
                                EmotePlatform = EmotePlatform.FrankerFaceZ,
                                Urls = urls
                            });
                        }
                    }
                }

                var combinedEmotes = bttvChannelEmotes.Select(bttvEmote => new CachedEmote
                        {Name = bttvEmote.Code, Urls = new List<Uri> {
                            new($"{_bttvCdnBaseUri}/{bttvEmote.Id}/1x"),
                            new($"{_bttvCdnBaseUri}/{bttvEmote.Id}/2x"),
                            new($"{_bttvCdnBaseUri}/{bttvEmote.Id}/3x")
                        }})
                    .ToList();
                combinedEmotes.AddRange(bttvSharedEmotes.Select(sharedEmote => new CachedEmote {
                    Name = sharedEmote.Code,
                    Urls = new List<Uri> {
                        new ($"{_bttvCdnBaseUri}/{sharedEmote.Id}/1x"),
                        new ($"{_bttvCdnBaseUri}/{sharedEmote.Id}/2x"),
                        new ($"{_bttvCdnBaseUri}/{sharedEmote.Id}/3x")
                    }
                }));
                combinedEmotes.AddRange(ffzChannelEmotes);
                cachedEmotes.AddRange(combinedEmotes);
                _emoteDict.Add(channelId, combinedEmotes);
            }

            return cachedEmotes;
        }
    }
}