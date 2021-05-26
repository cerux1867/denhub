namespace Denhub.Common {
    public class QueueSettings {
        public string QueueName { get; init; }
        public string ExchangeName { get; init; }
        public string ConnectionString { get; init; }
        public bool FairDispatchEnabled { get; }
        public int MaxReconnectAttempts { get; }
        public int IncrementalDelay { get; }

        public QueueSettings() {
            QueueName = "chat-messages";
            ExchangeName = "denhub-exchange";
            FairDispatchEnabled = true;
            MaxReconnectAttempts = 5;
            IncrementalDelay = 5;
        }
    }
}