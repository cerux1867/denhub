using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Models.Twitch;
using Denhub.Common;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denhub.API.Services {
    public class TwitchClient : ITwitchClient {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TwitchClient> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public TwitchClient(ILogger<TwitchClient> logger, HttpClient client, IOptions<TwitchSettings> settings) {
            _logger = logger;
            _httpClient = client;
            client.BaseAddress = new Uri("https://api.twitch.tv/helix/");
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {settings.Value.Token}");
            client.DefaultRequestHeaders.Add("Client-Id", settings.Value.ClientId);
            _jsonSerializerOptions = new JsonSerializerOptions {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
                Converters = {
                    new JsonStringEnumConverter(SnakeCaseNamingPolicy.Instance)
                }
            };
        }

        public async Task<TwitchResponseModel<IEnumerable<TwitchVideoItem>>> GetVideosByUserIdAsync(int userId,
            string paginationCursor = null, int limit = 100,
            TwitchVideoItemType type = TwitchVideoItemType.All) {
            var textVideoType = type switch {
                TwitchVideoItemType.Archive => "archive",
                TwitchVideoItemType.Highlight => "highlight",
                TwitchVideoItemType.Upload => "upload",
                TwitchVideoItemType.All => "all",
                _ => "all"
            };
            var queryParams = new Dictionary<string, string> {
                {"user_id", userId.ToString()},
                {"type", textVideoType},
                {"first", limit.ToString()}
            };

            if (!string.IsNullOrEmpty(paginationCursor)) {
                queryParams["after"] = paginationCursor;
            }

            var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString("videos", queryParams));
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"Request for videos failed with status code {response.StatusCode}.");
            }

            return JsonSerializer.Deserialize<TwitchResponseModel<IEnumerable<TwitchVideoItem>>>(
                await response.Content.ReadAsStringAsync(), _jsonSerializerOptions);
        }

        public async Task<TwitchResponseModel<IEnumerable<TwitchUserItem>>> GetUsersAsync(
            IEnumerable<string> loginNames) {
            var enumeratedLoginNames = loginNames.ToList();
            if (enumeratedLoginNames.Count > 100) {
                throw new ArgumentOutOfRangeException(nameof(loginNames), "Maximum number of login names is 100.");
            }

            var queryParams = enumeratedLoginNames.ToDictionary(loginName => "login");
            var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString("users", queryParams));
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"Request for users failed with status code {response.StatusCode}.");
            }

            return JsonSerializer.Deserialize<TwitchResponseModel<IEnumerable<TwitchUserItem>>>(
                await response.Content.ReadAsStringAsync(), _jsonSerializerOptions);
        }
    }
}