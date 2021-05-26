using System;
using System.IO;
using System.Threading.Tasks;
using Denhub.Chat.CollectorService.Models;
using Denhub.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace Denhub.Chat.CollectorService {
    public class Program {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                true, true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) => {
                    services.Configure<TwitchBotSettings>(hostContext.Configuration.GetSection("TwitchBotSettings"));
                    services.Configure<QueueSettings>(hostContext.Configuration.GetSection("Queue"));

                    services.AddSingleton(provider => {
                        var settings = provider.GetRequiredService<IOptions<QueueSettings>>();
                        var factory = new ConnectionFactory {
                            Uri = new Uri(settings.Value.ConnectionString),
                            ClientProvidedName = "app:denhub component:chat-collector"
                        };
                        IConnection connection = null;
                        var reconnectAttempts = 0;
                        var incrementalDelay = 0;

                        do {
                            try {
                                Task.Delay(1000 * incrementalDelay).Wait();
                                connection = factory.CreateConnection();
                            }
                            catch (BrokerUnreachableException ex) {
                                incrementalDelay += 5;
                                if (reconnectAttempts <= settings.Value.MaxReconnectAttempts) {
                                    reconnectAttempts++;
                                    Log.Logger.Warning(ex,
                                        "RabbitMQ endpoint unreachable and failed with exception, retrying in {Delay}ms",
                                        1000 * incrementalDelay);
                                }
                                else {
                                    Log.Logger.Error(ex, "RabbitMQ endpoint unreachable");
                                    throw;
                                }
                            }
                        } while (connection == null);

                        return connection;
                    });
                    services.AddSingleton<IChatMessageAsyncQueue, RabbitMqQueue>();
                    services.AddSingleton<ITwitchChatBot, TwitchChatBot>();

                    services.AddHostedService<TwitchBotWorker>();
                });
    }
}