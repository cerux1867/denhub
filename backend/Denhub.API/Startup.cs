using System.Net.Http;
using Denhub.API.Models;
using Denhub.API.Services;
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
            services.Configure<TwitchClientSettings>(options =>
                Configuration.GetSection("TwitchClientSettings").Bind(options));

            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins");
            var allowedOriginsList = allowedOrigins.Get<string[]>() ?? new [] {"*"};
            
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
            services.AddTransient<IVodsService, VodsService>();
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Denhub.API", Version = "v1"});
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