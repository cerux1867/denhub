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
using Denhub.Common.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
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
            services.Configure<MongoDbOptions>(options => {
                Configuration.GetSection("Database:MongoDB").Bind(options);
            });

            var dbVendor = Configuration.GetValue("Database:Vendor", "MongoDB");
            switch (dbVendor) {
                case "MongoDB":
                    services.AddSingleton<IMongoClient, MongoClient>(serviceProvider => {
                        var dbConnString = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value
                            .ConnectionString;
                        var client = new MongoClient(dbConnString);
                        return client;
                    });
                    services.AddSingleton<IChatLogsRepository, ChatLogsRepository>();
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
            services.AddTransient<IConnectionMultiplexer, ConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(Configuration.GetValue("Redis:ConfigString", "localhost:6379")));
            services.AddTransient<ITwitchClient, TwitchClient>();
            services.AddTransient<IVodRepository, RedisVodRepository>();
            services.AddTransient<ILogsService, MongoChatLogsService>();
            services.AddTransient<IVodsService, VodsService>();
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Denhub Platform API",
                    Version = "v1",
                    Description = "A RESTful API providing core Platform functionality for Denhub",
                    Contact = new OpenApiContact {
                        Email = string.Empty,
                        Url = new Uri("https://github.com/cerux1867"),
                        Name = "cerux1867 (Maintainer)"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            if (env.IsDevelopment() || Configuration.GetValue("Swagger:Enabled", false)) {
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