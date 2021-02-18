using Pilot2.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Pilot2.Services.Storage
{
	class StorageService : IStorageService
	{
		public ICollection<GameResultItem> LoadedData;

		private readonly string _storagePath;

		public StorageService(string storagePath)
		{
			_storagePath = storagePath;
		}

		public async Task AddGameResultsAsync(IEnumerable<GameResultItem> gameResultItems)
		{
			var data = (LoadedData != null && LoadedData.Count != 0) ? LoadedData.Union(gameResultItems) : gameResultItems;
			File.WriteAllText(_storagePath, JsonConvert.SerializeObject(data, Formatting.Indented));
		}

		public async Task<ICollection<GameResultItem>> GetGameResultsAsync()
		{
			ICollection<GameResultItem> collection;
			if (File.Exists(_storagePath))
            {
                var settings = new JsonSerializerSettings() { Formatting = Formatting.Indented };

                collection = JsonConvert.DeserializeObject<ICollection<GameResultItem>>(File.ReadAllText(_storagePath), settings);
			}
			else
			{
				File.Create(_storagePath).Close();
				collection = new Collection<GameResultItem>();
			}

			return collection;
		}

		public int GetScore(string name)
		{
			int score = 0;
			if (LoadedData != null && LoadedData.Count != 0)
			{
				foreach (var user in LoadedData)
				{
					if (user.UserName == name)
					{
						score += user.Score;
					}
				}
			}
			return score;
		}

		public ICollection<GameResultItem> GetLoadedData() => LoadedData;
	}
}
