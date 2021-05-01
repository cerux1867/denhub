using System;
using System.IO;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Denhub.Chat.Processor.Caches;
using Denhub.Chat.Processor.Processors;
using Denhub.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client;
using Serilog;

namespace Denhub.Chat.Processor {
    public class Program {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json",
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
                .ConfigureServices((context, services) => {
                    services.Configure<QueueSettings>(context.Configuration.GetSection("Queue"));

                    var dbVendor = context.Configuration.GetValue("Database:Vendor", "MongoDB");
                    switch (dbVendor) {
                        case "MongoDB":
                            var dbConnString =
                                context.Configuration.GetValue("Database:MongoDB:ConnectionString", "mongodb://localhost:27017");
                            services.AddSingleton<IMongoClient, MongoClient>(_ => {
                                var client = new MongoClient(dbConnString);
                                return client;
                            });
                            services.AddSingleton<IChatMessageRepository, MongoDbChatMessageRepository>();
                            break;
                        case "DynamoDB":
                            services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>(_ => {
                                var accessKey = Configuration.GetValue("Database:DynamoDB:AccessKey", "");
                                var secretKey = Configuration.GetValue("Database:DynamoDB:SecretKey", "");

                                if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey)) {
                                    throw new Exception(
                                        "AWS access and secret keys are not specified, unable to connect to a DynamoDB instance");
                                }
                                var basicCreds = new BasicAWSCredentials(accessKey, secretKey);

                                var dynamoDbConfig = new AmazonDynamoDBConfig();
                                var serviceUrl = Configuration.GetValue("Database:DynamoDB:ServiceUrl", "");
                                var regionEndpoint =
                                    Configuration.GetValue("Database:DynamoDB:RegionEndpoint", "");
                                if (!string.IsNullOrEmpty(serviceUrl)) {
                                    dynamoDbConfig.ServiceURL = serviceUrl;
                                } else if (!string.IsNullOrEmpty(regionEndpoint)) {
                                    dynamoDbConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(regionEndpoint);
                                }
                                else {
                                    throw new Exception(
                                        "No region or service URL specified, unable to connect to a DynamoDB instance");
                                }
                                    
                                var client = new AmazonDynamoDBClient(basicCreds, dynamoDbConfig);
                                return client;
                            });
                            services.AddSingleton<IChatMessageRepository, DynamoDbChatMessageRepository>();
                            break;
                    }
                    
                    services.AddSingleton(provider => {
                        var settings = provider.GetRequiredService<IOptions<QueueSettings>>();
                        var factory = new ConnectionFactory {
                            Uri = new Uri(settings.Value.ConnectionString),
                            ClientProvidedName = "app:denhub component:chat-processor-worker",
                            DispatchConsumersAsync = true
                        };

                        return factory.CreateConnection();
                    });

                    services.AddHttpClient<IEmoteCache, InMemoryEmoteCache>();
                    services.AddHttpClient<IBadgeCache, InMemoryBadgeCache>();
                    
                    services.AddSingleton<IBadgeProcessor, BadgeProcessor>();
                    services.AddSingleton<IEmoteProcessor, EmoteProcessor>();
                    services.AddSingleton<IChatMessagePreprocessor, ChatMessagePreprocessor>();
                    
                    services.AddHostedService<ChatMessageProcessingWorker>();
                });
    }
}