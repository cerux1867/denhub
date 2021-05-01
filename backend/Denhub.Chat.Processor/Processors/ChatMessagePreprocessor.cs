using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Denhub.Chat.Processor.Models;

namespace Denhub.Chat.Processor.Processors {
    public class ChatMessagePreprocessor : IChatMessagePreprocessor {
        private const string RegexPattern =
            @"@badge-info=([^;]*).*badges=(.*\/[0-9]*)?.*color=([^;]*).*display-name=([^;]*).*emotes=([^;]*).*id=([^;]*).*room-id=([0-9]*);.*subscriber=([01]);.*tmi-sent-ts=([0-9]*);.*user-id=([0-9]*);.*:(.*)!.*@.*\sPRIVMSG\s#([^\s:]*)\s:(.*)";
        private readonly Regex _regex;

        public ChatMessagePreprocessor() {
            _regex = new Regex(RegexPattern, RegexOptions.Compiled);
        }
        
        public TwitchChatMessage ProcessMessage(string message) {
            var match = _regex.Match(message);
            var groups = match.Groups;
            var partiallyProcessedTwitchMessage = new TwitchChatMessage {
                RawBadges = groups[2].Captures.Count > 0 ? groups[2].Captures[0].Value.Split(",") : Array.Empty<string>(),
                UserColor = groups[3].Captures.Count > 0 && !string.IsNullOrEmpty(groups[3].Captures[0].Value) ? groups[3].Captures[0].Value : "#858585",
                UserDisplayName = groups[4].Captures.Count > 0 ? groups[11].Captures[0].Value : groups[4].Captures[0].Value,
                Emotes = groups[5].Captures.Count > 0 && !string.IsNullOrEmpty(groups[5].Captures[0].Value) ? ProcessEmotes(groups[5].Captures[0].Value) : Array.Empty<TwitchEmote>().ToList(),
                MessageId = groups[6].Captures[0].Value,
                ChannelId = Convert.ToInt64(groups[7].Captures[0].Value),
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(groups[9].Captures[0].Value)).UtcDateTime,
                UserId = Convert.ToInt64(groups[10].Captures[0].Value),
                ChannelDisplayName = groups[12].Captures[0].Value,
                Message = groups[13].Captures[0].Value
            };

            return partiallyProcessedTwitchMessage;
        }

        private List<TwitchEmote> ProcessEmotes(string emoteString) {
            var uniqueEmotes = emoteString.Split("/");
            var emotes = new List<TwitchEmote>();
            foreach (var emote in uniqueEmotes) {
                var splitEmote = emote.Split(":");
                var emoteId = splitEmote[0];
                var indices = splitEmote[1].Split(",");
                foreach (var indexPair in indices) {
                    var splitIndices = indexPair.Split("-");
                    var parsedEmote = new TwitchEmote {
                        EmoteUrl = new List<Uri> {
                            new ($"https://static-cdn.jtvnw.net/emoticons/v1/{emoteId}/1.0"),
                            new ($"https://static-cdn.jtvnw.net/emoticons/v1/{emoteId}/2.0"),
                            new ($"https://static-cdn.jtvnw.net/emoticons/v1/{emoteId}/3.0"),
                            new ($"https://static-cdn.jtvnw.net/emoticons/v1/{emoteId}/4.0")
                        },
                        StartIndex = Convert.ToInt32(splitIndices[0]),
                        EndIndex = Convert.ToInt32(splitIndices[1])
                    };
                    emotes.Add(parsedEmote);
                }
            }

            return emotes;
        }
    }
}