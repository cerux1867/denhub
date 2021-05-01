namespace Denhub.Common {
    public class QueueSettings {
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string ConnectionString { get; set; }
        public bool FairDispatchEnabled { get; set; }

        public QueueSettings() {
            QueueName = "chat-messages";
            ExchangeName = "denhub-exchange";
            FairDispatchEnabled = true;
        }
    }
}