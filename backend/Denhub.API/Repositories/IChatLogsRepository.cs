using System.Linq;
using System.Threading.Tasks;
using Denhub.Common.Models;
using MongoDB.Driver.Linq;

namespace Denhub.API.Repositories {
    public interface IChatLogsRepository {
        /// <summary>
        /// Asynchronously retrieves items as a <see cref="IMongoQueryable{T}"/>
        /// </summary>
        /// <returns>A LINQ-compatible MongoDB implementation of <see cref="IQueryable{T}"/> - <see cref="IMongoQueryable{T}"/></returns>
        public Task<IMongoQueryable<TwitchChatMessage>> GetAllAsync();
        
        /// <summary>
        /// Inserts an item into the database
        /// </summary>
        /// <param name="item">New item</param>
        public Task InsertAsync(TwitchChatMessage item);
    }
}