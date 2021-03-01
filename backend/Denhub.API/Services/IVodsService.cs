using System.Collections.Generic;
using System.Threading.Tasks;
using Denhub.API.Models;

namespace Denhub.API.Services {
    public interface IVodsService {
        public Task<int> GetChannelIdByChannelNameAsync(string channelName);
        public Task<IEnumerable<CommonVodModel>> GetAllByIdAsync(int channelId, int page = 1, int limit = 10, string titleFilter = "");
    }
}