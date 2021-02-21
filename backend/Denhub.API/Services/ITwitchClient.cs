using System.Collections.Generic;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Models.Twitch;

namespace Denhub.API.Services {
    public interface ITwitchClient {
        public Task<TwitchResponseModel<IEnumerable<TwitchVideoItem>>> GetVideosByUserIdAsync(int userId,
            string paginationCursor = null, int limit = 100,
            TwitchVideoItemType type = TwitchVideoItemType.All);

        public Task<TwitchResponseModel<IEnumerable<TwitchUserItem>>> GetUsersAsync(IEnumerable<string> loginNames);
    }
}