using System;

namespace Denhub.Chat.Collector.Models {
    public class TwitchBotSettings {
        public string BotUsername { get; set; }
        public string BotPassword { get; set; }
        public string[] ConfiguredChannels { get; set; }

        public TwitchBotSettings() {
            ConfiguredChannels = Array.Empty<string>();
        }
    }
}