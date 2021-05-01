﻿using System;
using System.Threading.Tasks;
using Denhub.Chat.Collector.Models;
using Denhub.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;

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
            appLifetime.ApplicationStopping.Register(() => {
                bot.Dispose();
            });
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((context, services) => {
                services.Configure<TwitchBotSettings>(context.Configuration.GetSection("TwitchBotSettings"));
                services.Configure<QueueSettings>(context.Configuration.GetSection("Queue"));
                
                services.AddSingleton(provider => {
                    var settings = provider.GetRequiredService<IOptions<QueueSettings>>();
                    var factory = new ConnectionFactory {
                        Uri = new Uri(settings.Value.ConnectionString),
                        ClientProvidedName = "app:denhub component:chat-collector"
                    };

                    return factory.CreateConnection();
                });
                services.AddSingleton<IChatMessageAsyncQueue, RabbitMqQueue>();
                services.AddSingleton<ITwitchChatBot, TwitchChatBot>();
            });
    }
}