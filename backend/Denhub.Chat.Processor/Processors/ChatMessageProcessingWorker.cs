using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Denhub.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Denhub.Chat.Processor.Processors {
    public class ChatMessageProcessingWorker : BackgroundService {
        private readonly ILogger<ChatMessageProcessingWorker> _logger;
        private readonly IConnection _connection;
        private IModel _channel;
        private readonly IOptions<QueueSettings> _options;
        private const string RoutingKey = "denhub-chat-messages";
        private readonly Guid _workerGuid;
        private readonly IChatMessagePreprocessor _preprocessor;
        private readonly IChatMessageRepository _repository;
        private readonly IEmoteProcessor _emoteProcessor;
        private readonly IBadgeProcessor _badgeProcessor;

        public ChatMessageProcessingWorker(ILogger<ChatMessageProcessingWorker> logger, IConnection connection,
            IOptions<QueueSettings> settings, IChatMessagePreprocessor preprocessor,
            IChatMessageRepository repository, IEmoteProcessor emoteProcessor, IBadgeProcessor badgeProcessor) {
            _logger = logger;
            _connection = connection;
            _options = settings;
            _preprocessor = preprocessor;
            _workerGuid = Guid.NewGuid();
            _repository = repository;
            _emoteProcessor = emoteProcessor;
            _badgeProcessor = badgeProcessor;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            _channel = _connection.CreateModel();

            // Defines exchange
            _channel.ExchangeDeclare(_options.Value.ExchangeName, ExchangeType.Direct, true);
            // Defines and binds a queue to receive messages
            _channel.QueueDeclare(_options.Value.QueueName, true, false, false);
            _channel.QueueBind(_options.Value.QueueName, _options.Value.ExchangeName, RoutingKey);

            _logger.LogInformation("Worker {WorkerGuid} now listening for messages on queue {QueueName} at {RabbitMqEndpoint}", _workerGuid, _options.Value.QueueName, _connection.Endpoint);

            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken) {
            await base.StopAsync(cancellationToken);
            _connection.Close();
            _logger.LogInformation("Worker {WorkerGuid} stopped", _workerGuid);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, eventArgs) => {
                var rawMessage = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                try {
                    var message = JsonSerializer.Deserialize<UnprocessedChatMessage>(rawMessage);
                    if (message == null) {
                        _channel.BasicNack(eventArgs.DeliveryTag, false, false);
                    }
                    else {
                        var preprocessedMessage = _preprocessor.ProcessMessage(message.RawChatMessage);

                        if (string.IsNullOrEmpty(preprocessedMessage.MessageId)) {
                            _logger.LogError("Message lacked required parameter MessageId, discarding. Message: {Message}", message.RawChatMessage);
                        }
                        else {
                            var badges = await _badgeProcessor.ProcessAsync(preprocessedMessage.ChannelId, preprocessedMessage.RawBadges.ToList());
                            var enrichedMessage = await _emoteProcessor.EnrichWithExternalEmotesAsync(preprocessedMessage);
                            enrichedMessage.Badges = badges;
                        
                            await _repository.AddAsync(enrichedMessage);
                        }
                        _channel.BasicAck(eventArgs.DeliveryTag, false);
                    }
                }
                catch (JsonException ex) {
                    _logger.LogError(ex, "JSON parse error: {Error}", ex.Message);
                    _channel.BasicNack(eventArgs.DeliveryTag, false, false);
                }
                catch (AlreadyClosedException) {
                    _logger.LogInformation(
                        "Worker {WorkerGuid} is no longer listening for messages and cannot process message {DeliveryTag}",
                        _workerGuid, eventArgs.DeliveryTag);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Unknown exception occured {ErrorMessage}", ex.Message);
                }
            };

            _channel.BasicConsume(_options.Value.QueueName, false, consumer);

            await Task.CompletedTask;
        }
    }
}