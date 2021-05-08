using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Denhub.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Denhub.Chat.Collector {
    public class RabbitMqQueue : IChatMessageAsyncQueue, IDisposable {
        private readonly IConnection _rabbitMqConnection;
        private readonly IModel _channel;
        private readonly IOptions<QueueSettings> _settings;
        private const string RoutingKey = "denhub-chat-messages";

        public RabbitMqQueue(ILogger<RabbitMqQueue> logger, IOptions<QueueSettings> settings, IConnection rabbitMqConnection) {
            _rabbitMqConnection = rabbitMqConnection;
            _settings = settings;
            _channel = _rabbitMqConnection.CreateModel();
            _channel.ExchangeDeclare(settings.Value.ExchangeName, ExchangeType.Direct, true);
            _channel.QueueDeclare(settings.Value.QueueName, true, false, false);
            if (_settings.Value.FairDispatchEnabled) {
                _channel.BasicQos(0, 1, false);  
            }
            _channel.QueueBind(settings.Value.QueueName, settings.Value.ExchangeName, RoutingKey);
            logger.LogInformation("Connected to queue {QueueName} at {RabbitMqEndpoint}", settings.Value.QueueName, _rabbitMqConnection.Endpoint);
        }
        
        public Task EnqueueAsync(UnprocessedChatMessage message) {
            var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var basicProps = _channel.CreateBasicProperties();
            basicProps.ContentType = "application/json";
            basicProps.DeliveryMode = 2;
            basicProps.Persistent = true;
            _channel.BasicPublish(_settings.Value.ExchangeName, RoutingKey, basicProps, messageBytes);
            return Task.CompletedTask;
        }

        public void Dispose() {
            _rabbitMqConnection?.Dispose();
            _channel?.Dispose();
        }
    }
}