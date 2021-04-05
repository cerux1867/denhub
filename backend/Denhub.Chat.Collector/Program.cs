using System.Threading.Tasks;
using Denhub.Chat.Collector.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;

namespace Denhub.Chat.Collector {
    class Program {
        public static async Task Main(string[] args) {
            var host = CreateHostBuilder(args).Build();
            var config = host.Services.GetRequiredService<IConfiguration>();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
            var bot = host.Services.GetRequiredService<ITwitchChatBot>();
            var queue = host.Services.GetRequiredService<IChatMessageAsyncQueue>();
            await bot.ConnectAsync();
            bot.MessageReceived += (_, eventArgs) => {
                queue.EnqueueAsync(eventArgs.UnprocessedMessage);
            };
            var appLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            appLifetime.ApplicationStopping.Register(async () => {
                bot.Dispose();
            });
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((context, services) => {
                services.Configure<TwitchBotSettings>(context.Configuration.GetSection("TwitchBotSettings"));
                services.Configure<RedisSettings>(context.Configuration.GetSection("Redis"));
                
                services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(provider  => {
                    var redisSettings = provider.GetRequiredService<IOptions<RedisSettings>>();
                    return ConnectionMultiplexer.Connect(redisSettings.Value.ConfigString);
                });
                services.AddSingleton<IChatMessageAsyncQueue, RedisChatMessageQueue>();
                services.AddSingleton<ITwitchChatBot, TwitchChatBot>();
            });
    }
}