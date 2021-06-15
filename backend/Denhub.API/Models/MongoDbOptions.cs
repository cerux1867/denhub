namespace Denhub.API.Models {
    public class MongoDbOptions {
        public string DatabaseName { get; set; }
        public string ChatLogsCollectionName { get; set; }
        public string ConnectionString { get; set; }

        public MongoDbOptions() {
            DatabaseName = "denhub";
            ChatLogsCollectionName = "chat_messages";
            ConnectionString = "mongodb://localhost:27017";
        }
    }
}