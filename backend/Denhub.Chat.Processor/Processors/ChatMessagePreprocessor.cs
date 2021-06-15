using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Denhub.Common.Models;

namespace Denhub.Chat.Processor.Processors {
    public class ChatMessagePreprocessor : IChatMessagePreprocessor {
        private const string RegexPattern =
            @"(.*)!.*@.*\sPRIVMSG\s#([^\s:]*)\s:(.*)";
        private readonly Regex _regex;

        public ChatMessagePreprocessor() {
            _regex = new Regex(RegexPattern, RegexOptions.Compiled);
        }
        
        public TwitchChatMessage ProcessMessage(string message) {
            var partiallyProcessedTwitchMessage = new TwitchChatMessage();

            var splitMsg = message.Split(" :", 2);
            var tags = splitMsg[0].Split(';');
            foreach (var tag in tags) {
                var splitTag = tag.Split('=');
                switch (splitTag[0]) {
                    case "badges":
                        partiallyProcessedTwitchMessage.RawBadges = !string.IsNullOrEmpty(splitTag[1])
                            ? splitTag[1].Split(",")
                            : Array.Empty<string>();
                        break;
                    case "color":
                        partiallyProcessedTwitchMessage.UserColor =
                            !string.IsNullOrEmpty(splitTag[1]) ? splitTag[1] : "#858585";
                        break;
                    case "display-name":
                        partiallyProcessedTwitchMessage.UserDisplayName =
                            !string.IsNullOrEmpty(splitTag[1]) ? splitTag[1] : string.Empty;
                        break;
                    case "emotes":
                        partiallyProcessedTwitchMessage.Emotes = !string.IsNullOrEmpty(splitTag[1])
                            ? ProcessEmotes(splitTag[1])
                            : Array.Empty<TwitchEmote>().ToList();
                        break;
                    case "id":
                        partiallyProcessedTwitchMessage.MessageId = splitTag[1];
                        break;
                    case "room-id":
                        partiallyProcessedTwitchMessage.ChannelId = Convert.ToInt64(splitTag[1]);
                        break;
                    case "tmi-sent-ts":
                        partiallyProcessedTwitchMessage.Timestamp = Convert.ToInt64(splitTag[1]);
                        break;
                    case "user-id":
                        partiallyProcessedTwitchMessage.UserId = Convert.ToInt64(splitTag[1]);
                        break;
                }
            }
            var match = _regex.Match(splitMsg[1]);
            var groups = match.Groups;
            if (string.IsNullOrEmpty(partiallyProcessedTwitchMessage.UserDisplayName)) {
                partiallyProcessedTwitchMessage.UserDisplayName = groups[1].Captures[0].Value;
            }

            partiallyProcessedTwitchMessage.ChannelDisplayName = groups[2].Captures[0].Value;
            partiallyProcessedTwitchMessage.Message = groups[3].Captures[0].Value;

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
                        EmoteUrl = new List<string> {
                            $"https://static-cdn.jtvnw.net/emoticons/v1/{emoteId}/1.0",
                            $"https://static-cdn.jtvnw.net/emoticons/v1/{emoteId}/2.0",
                            $"https://static-cdn.jtvnw.net/emoticons/v1/{emoteId}/3.0",
                            $"https://static-cdn.jtvnw.net/emoticons/v1/{emoteId}/4.0"
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