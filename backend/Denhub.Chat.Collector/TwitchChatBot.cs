using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Denhub.Chat.Collector.Models;
using Denhub.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denhub.Chat.Collector {
    public class TwitchChatBot : ITwitchChatBot {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private readonly IOptions<TwitchBotSettings> _twitchBotSettings;
        private TcpClient _tcpClient;
        private readonly ILogger<TwitchChatBot> _logger;
        private Thread _receiveMessageThread;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _internalCancellationTokenSource;
        private EventHandler _reconnectRequested;
        private int _reconnectAttempts;
        private int _incrementalDelay;

        public TwitchChatBot(IOptions<TwitchBotSettings> botSettings, ILogger<TwitchChatBot> logger) {
            _twitchBotSettings = botSettings;
            _incrementalDelay = 0;
            _tcpClient = new TcpClient();
            _logger = logger;
            _reconnectRequested += async (_, eventArgs) => {
                if (_reconnectAttempts <= botSettings.Value.MaxReconnectAttempts) {
                    _reconnectAttempts++;
                    if (_receiveMessageThread.IsAlive) {
                        _internalCancellationTokenSource?.Cancel();
                    }

                    Dispose();
                    _tcpClient = new TcpClient();
                    await Task.Delay(1000 * _incrementalDelay);
                    _incrementalDelay += botSettings.Value.IncrementalDelay;
                    await ConnectAsync();
                }
                else {
                    _logger.LogError("Failed to establish connection to the Twitch IRC server after {NumAttempts} attempts", _reconnectAttempts - 1);
                    throw new Exception("Failed to reconnect to the Twitch IRC server");
                }
            };
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

            foreach (var channel in _twitchBotSettings.Value.ConfiguredChannels) {
                await streamWriter.WriteLineAsync($"JOIN #{channel}");
                _logger.LogInformation("Listening on channel {Channel}", channel);
            }

            _receiveMessageThread = new Thread(() => ReadMessages(streamReader, streamWriter));
            _receiveMessageThread.Start();
        }

        private void ReadMessages(StreamReader reader, StreamWriter writer) {
            while (!_cancellationToken.IsCancellationRequested) {
                var line = string.Empty;
                try {
                    line = reader.ReadLine();
                }
                catch (IOException ex) {
                    if (_reconnectAttempts <= _twitchBotSettings.Value.MaxReconnectAttempts) {
                        _reconnectRequested.Invoke(this, EventArgs.Empty);
                        _logger.LogWarning("Failed to read from the Twitch TCP socket, reconnecting in {Delay}ms", 1000 * _incrementalDelay);
                        break;
                    }
                    
                    if (_reconnectAttempts > _twitchBotSettings.Value.MaxReconnectAttempts) {
                        _logger.LogError(ex, "Failed to read from the Twitch TCP socket");
                        throw;
                    }
                }
                if (string.IsNullOrEmpty(line)) {
                    _logger.LogWarning("Empty message line received, reconnecting in {Delay}ms", 1000 * _incrementalDelay);
                    _reconnectRequested.Invoke(this, EventArgs.Empty);
                    break;
                }
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