using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Denhub.Chat.Collector {
    public class TwitchBotWorker : BackgroundService {
        private readonly ITwitchChatBot _twitchChatBot;
        private readonly IChatMessageAsyncQueue _chatMessageAsyncQueue;
        
        public TwitchBotWorker(ITwitchChatBot twitchChatBot, IChatMessageAsyncQueue queue) {
            _twitchChatBot = twitchChatBot;
            _chatMessageAsyncQueue = queue;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            _twitchChatBot.MessageReceived += async (_, eventArgs) => {
                await _chatMessageAsyncQueue.EnqueueAsync(eventArgs.UnprocessedMessage);
            };
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            _twitchChatBot.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await _twitchChatBot.ConnectAsync(stoppingToken);
        }
    }
}