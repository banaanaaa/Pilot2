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
		private readonly ICollection<GameResultItem> _loadedData;
		private readonly string _storagePath;

		public StorageService(string storagePath)
		{
			_storagePath = storagePath;
			_loadedData = Load();
		}

		public Task AddGameResultsAsync(IEnumerable<GameResultItem> gameResultItems)
		{
			return Task.Run(() => Save(gameResultItems));
		}

		public Task<ICollection<GameResultItem>> GetGameResultsAsync(Guid userid)
		{
			return Task.Run(() => Load(userid));
		}

		public Task<ICollection<GameResultItem>> GetGameResultsAsync()
		{
			return Task.Run(() => Load());
		}

		private void Save(IEnumerable<GameResultItem> gameResultItems)
		{
			/*using (StreamWriter sw = new StreamWriter(_storagePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, gameResultItems);
            }*/


			File.WriteAllText(@_storagePath, JsonConvert.SerializeObject(gameResultItems));
/*
			using (StreamWriter file = File.AppendText(@_storagePath))
			{
				string json = JsonConvert.SerializeObject(gameResultItems, Formatting.Indented);
				*//*var settings = new JsonSerializerSettings() { Formatting = Formatting.Indented };

				string data = JsonConvert.SerializeObject(gameResultItems, settings);*//*
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(file, json);
			}*/


        }

        private ICollection<GameResultItem> Load(Guid userid)
		{
			ICollection<GameResultItem> collection;
			if (File.Exists(_storagePath))
			{/*
				var settings = new JsonSerializerSettings() { Formatting = Formatting.Indented };*/

				collection = (ICollection<GameResultItem>)JsonConvert.DeserializeObject<ICollection<GameResultItem>>(File.ReadAllText(_storagePath)/*, settings*/)
					.Select(t => t.UserId == userid);
			}
			else
			{
				File.Create(_storagePath);
				collection = new Collection<GameResultItem>();
			}
			return collection;
		}

		private ICollection<GameResultItem> Load()
		{
			ICollection<GameResultItem> collection;
			if (File.Exists(_storagePath))
            {
                var settings = new JsonSerializerSettings() { Formatting = Formatting.Indented };

                collection = JsonConvert.DeserializeObject<ICollection<GameResultItem>>(File.ReadAllText(_storagePath), settings);
			}
			else
			{
				File.Create(_storagePath);
				collection = new Collection<GameResultItem>();
			}
			return collection;
		}

		public int GetScore(Guid userid)
		{
			if (_loadedData.Count == 0)
			{
				throw new ArgumentException("the data is empty or not loaded");
			}
			int tmp = 0;
			foreach (var user in _loadedData)
			{
				if (user.UserId == userid)
					tmp = user.Score;
			}
			return tmp;
		}
	}
}
