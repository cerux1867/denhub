using System;
using System.Threading;
using System.Threading.Tasks;
using Denhub.Chat.Collector.Models;

namespace Denhub.Chat.Collector {
    public interface ITwitchChatBot : IDisposable {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public Task ConnectAsync();
        public Task ConnectAsync(CancellationToken cancellationToken);
    }
}