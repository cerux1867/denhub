using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Denhub.API.Models;
using Denhub.API.Repositories;
using Denhub.API.Services;
using Denhub.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace Denhub.API {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<TwitchSettings>(options =>
                Configuration.GetSection("TwitchClientSettings").Bind(options));

            var dbVendor = Configuration.GetValue("Database:Vendor", "MongoDB");
            switch (dbVendor) {
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
                        }
                        else if (!string.IsNullOrEmpty(regionEndpoint)) {
                            dynamoDbConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(regionEndpoint);
                        }
                        else {
                            throw new Exception(
                                "No region or service URL specified, unable to connect to a DynamoDB instance");
                        }

                        var client = new AmazonDynamoDBClient(basicCreds, dynamoDbConfig);
                        return client;
                    });
                    services.AddSingleton<IChatLogsRepository, DynamoDbChatLogsRepository>();
                    break;
            }

            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins");
            var allowedOriginsList = allowedOrigins.Get<string[]>() ?? new[] {"*"};

            services.AddCors(options => {
                options.AddPolicy("AllowedOrigins", builder => {
                    builder
                        .WithOrigins(allowedOriginsList)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddTransient<HttpClient>();
            services.AddTransient<IConnectionMultiplexer, ConnectionMultiplexer>(provider =>
                ConnectionMultiplexer.Connect(Configuration.GetValue("Redis:ConfigString", "localhost:6379")));
            services.AddTransient<ITwitchClient, TwitchClient>();
            services.AddTransient<IVodRepository, RedisVodRepository>();
            services.AddTransient<IChatLogsRepository, DynamoDbChatLogsRepository>();
            services.AddTransient<ILogsService, LogsService>();
            services.AddTransient<IVodsService, VodsService>();
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Denhub.API", Version = "v1"});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Denhub.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowedOrigins");

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}