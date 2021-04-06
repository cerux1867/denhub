namespace Denhub.Chat.Collector.Models {
    public class QueueSettings {
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string ConnectionString { get; set; }

        public QueueSettings() {
            QueueName = "chat-messages";
            ExchangeName = "denhub-exchange";
        }
    }
}