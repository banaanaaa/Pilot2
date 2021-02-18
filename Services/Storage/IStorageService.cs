using Pilot2.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pilot2.Services.Storage
{
    public interface IStorageService
    {
        //public Task<ICollection<GameResultItem>> GetGameResultsAsync(Guid userid);

        /// <summary>
        /// Get game results from somewhere
        /// </summary>
        /// <returns></returns>
        public Task<ICollection<GameResultItem>> GetGameResultsAsync();

        /// <summary>
        /// Save game reults somewhere
        /// </summary>
        /// <param name="gameResultItems"></param>
        /// <returns></returns>
        public Task AddGameResultsAsync(IEnumerable<GameResultItem> gameResultItems);

        public int GetScore(string name);

        public ICollection<GameResultItem> GetLoadedData();
    }
}
