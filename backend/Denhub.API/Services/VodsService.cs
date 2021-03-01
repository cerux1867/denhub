using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Models;
using Microsoft.Extensions.Logging;

namespace Denhub.API.Services {
    public class VodsService : IVodsService {
        private readonly ILogger<VodsService> _logger;
        private readonly IVodRepository _vodRepository;
        private readonly ITwitchClient _twitchClient;

        public VodsService(ILogger<VodsService> logger, IVodRepository vodRepository, ITwitchClient twitchClient) {
            _logger = logger;
            _vodRepository = vodRepository;
            _twitchClient = twitchClient;
        }

        public async Task<int> GetChannelIdByChannelNameAsync(string channelName) {
            var response = await _twitchClient.GetUsersAsync(new[] {channelName});
            if (response.Data.Any()) {
                return Convert.ToInt32(response.Data.First().Id);
            }

            return -1;
        }

        public async Task<IEnumerable<CommonVodModel>> GetAllByIdAsync(int channelId, int page = 1, int limit = 10, string titleFilter = "") {
            var startIndex = page == 1 ? 0 : (page - 1) * limit;
            return await _vodRepository.GetOrFetchVodsAsync(channelId, startIndex, limit, titleFilter);
        }
    }
}