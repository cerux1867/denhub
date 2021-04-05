using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Denhub.Chat.Collector.Models;
using Microsoft.Extensions.Options;

namespace Denhub.Chat.Collector {
    public class TwitchChatBot : ITwitchChatBot {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private readonly IOptions<TwitchBotSettings> _twitchSecurity;
        private readonly TcpClient _tcpClient;
        private Thread _receiveMessageThread;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _internalCancellationTokenSource;

        public TwitchChatBot(IOptions<TwitchBotSettings> security) {
            _twitchSecurity = security;
            _tcpClient = new TcpClient();
        }

        public async Task ConnectAsync() {
            _internalCancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _internalCancellationTokenSource.Token;
            await _tcpClient.ConnectAsync("irc.chat.twitch.tv", 6667, _cancellationToken);
            await CompleteConnectAsync();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken) {
            _cancellationToken = cancellationToken;
            await _tcpClient.ConnectAsync("irc.chat.twitch.tv", 6667, _cancellationToken);
            await CompleteConnectAsync();
        }

        private async Task CompleteConnectAsync() {
            var streamReader = new StreamReader(_tcpClient.GetStream());
            var streamWriter = new StreamWriter(_tcpClient.GetStream()) {
                NewLine = "\r\n", 
                AutoFlush = true
            };

            await streamWriter.WriteLineAsync($"PASS oauth:{_twitchSecurity.Value.BotPassword}");
            await streamWriter.WriteLineAsync($"NICK {_twitchSecurity.Value.BotUsername}");
            await streamWriter.WriteLineAsync("CAP REQ :twitch.tv/tags");

            // TODO: Join all configured Twitch channels
            await streamWriter.WriteLineAsync("JOIN #ludwig");

            _receiveMessageThread = new Thread(() => ReadMessages(streamReader, streamWriter));
            _receiveMessageThread.Start();
        }

        private void ReadMessages(StreamReader reader, StreamWriter writer) {
            while (!_cancellationToken.IsCancellationRequested) {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                var splitLine = line.Split(" ");
                    
                if (line.StartsWith("PING")) {
                    writer.WriteLine($"PONG {splitLine[1]}");
                }
                else if (line.Contains("PRIVMSG")) {
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs {
                        UnprocessedMessage = new UnprocessedChatMessage {
                            RawChatMessage = line,
                            TimeReceived = DateTime.UtcNow   
                        }
                    });
                }
            }

            _receiveMessageThread.Join();
        }

        public void Dispose() {
            _internalCancellationTokenSource?.Cancel();
            _internalCancellationTokenSource?.Dispose();
            _tcpClient?.Dispose();
        }
    }
}