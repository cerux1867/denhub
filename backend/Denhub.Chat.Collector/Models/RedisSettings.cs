namespace Denhub.Chat.Collector.Models {
    public class RedisSettings {
        public string QueueListKey { get; set; }
        public string ConfigString { get; set; }

        public RedisSettings() {
            QueueListKey = "denhub_chat_messages_queue";
            ConfigString = "localhost:6379";
        }
    }
}