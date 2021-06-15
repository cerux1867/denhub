using System.Collections.Generic;
using System.Threading.Tasks;
using Denhub.API.Models;

namespace Denhub.API.Repositories {
    public interface IVodRepository {
        /// <summary>
        /// Gets stored Vods 
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="titleFilter"></param>
        /// <returns></returns>
        public Task<IEnumerable<CommonVodModel>> GetOrFetchVodsAsync(int channelId, int offset = 0, int limit = 100, string titleFilter = "");
    }
}