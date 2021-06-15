using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Caches;
using Denhub.Chat.Processor.Models;
using Denhub.Common.Models;

namespace Denhub.Chat.Processor.Processors {
    public class EmoteProcessor : IEmoteProcessor {
        private readonly IEmoteCache _emoteCache;

        public EmoteProcessor(IEmoteCache emoteCache) {
            _emoteCache = emoteCache;
        }

        public async Task<TwitchChatMessage> EnrichWithExternalEmotesAsync(TwitchChatMessage chatMessageBackend) {
            var channelEmoteList = await _emoteCache.GetChannelCachedAsync(chatMessageBackend.ChannelId);
            foreach (var cachedEmote in channelEmoteList) {
                var pattern = $@"(?:^|\W)({cachedEmote.Name})(?:$|\W)";
                var matches = Regex.Matches(chatMessageBackend.Message, pattern);
                foreach (Match match in matches) {
                    if (match.Groups.Count > 1 && match.Groups[1].Captures.Count > 0) {
                        var msgEmotes = chatMessageBackend.Emotes.ToList();
                        foreach (Capture capture in match.Groups[1].Captures) {
                            // Check if emote already exists in the emote list, if it does, break the loop.
                            var existing = msgEmotes.FirstOrDefault(e =>
                                e.StartIndex == capture.Index && e.EndIndex == capture.Index + capture.Length - 1);
                            if (existing != null) {
                                continue;
                            }

                            var emote = new TwitchEmote {
                                EmoteUrl = cachedEmote.Urls,
                                StartIndex = capture.Index,
                                EndIndex = capture.Index + capture.Length - 1
                            };
                            chatMessageBackend.Emotes = msgEmotes.Append(emote).ToList();
                        }
                    }
                }
            }

            return chatMessageBackend;
        }
    }
}