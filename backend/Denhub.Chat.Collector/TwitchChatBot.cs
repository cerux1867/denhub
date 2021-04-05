using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Denhub.Chat.Collector.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denhub.Chat.Collector {
    public class TwitchChatBot : ITwitchChatBot {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private readonly IOptions<TwitchBotSettings> _twitchBotSettings;
        private readonly TcpClient _tcpClient;
        private readonly ILogger<TwitchChatBot> _logger;
        private Thread _receiveMessageThread;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _internalCancellationTokenSource;

        public TwitchChatBot(IOptions<TwitchBotSettings> botSettings, ILogger<TwitchChatBot> logger) {
            _twitchBotSettings = botSettings;
            _tcpClient = new TcpClient();
            _logger = logger;
        }

        public async Task ConnectAsync() {
            _internalCancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _internalCancellationTokenSource.Token;
            await CompleteConnectAsync();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken) {
            _cancellationToken = cancellationToken;
            await CompleteConnectAsync();
        }

        private async Task CompleteConnectAsync() {
            await _tcpClient.ConnectAsync("irc.chat.twitch.tv", 6667, _cancellationToken);
            var streamReader = new StreamReader(_tcpClient.GetStream());
            var streamWriter = new StreamWriter(_tcpClient.GetStream()) {
                NewLine = "\r\n", 
                AutoFlush = true
            };

            await streamWriter.WriteLineAsync($"PASS oauth:{_twitchBotSettings.Value.BotPassword}");
            await streamWriter.WriteLineAsync($"NICK {_twitchBotSettings.Value.BotUsername}");
            await streamWriter.WriteLineAsync("CAP REQ :twitch.tv/tags");

            // TODO: Join all configured Twitch channels
            foreach (var channel in _twitchBotSettings.Value.ConfiguredChannels) {
                await streamWriter.WriteLineAsync($"JOIN #{channel}");
            }

            _receiveMessageThread = new Thread(() => ReadMessages(streamReader, streamWriter));
            _receiveMessageThread.Start();
        }

        private void ReadMessages(StreamReader reader, StreamWriter writer) {
            while (!_cancellationToken.IsCancellationRequested) {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) {
                    _logger.LogWarning("Empty message line received, potential connectivity issues");
                    continue;
                };
                var splitLine = line.Split(" ");
                    
                if (line.StartsWith("PING")) {
                    writer.WriteLine($"PONG {splitLine[1]}");
                }
                else if (line.Contains("PRIVMSG")) {
                    if (line.Contains("\001ACTION")) {
                        continue;
                    }
                    _logger.LogDebug("Twitch chat message: {Message}", line);
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