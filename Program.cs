using Pilot2.Services.Play;
using Pilot2.Services.Storage;
using System.Threading.Tasks;

namespace Pilot2
{
	class Program
	{
		static async Task Main(string[] args)
		{
			StorageService storageService = new StorageService("../../gamelog.json");
			storageService.LoadedData = await storageService.GetGameResultsAsync();
			Game game = new Game(storageService);

			int count = game.InputNumberOfPlayers();
			
			game.InitPlayers(count);

			game.StartGame();

			while (game.SomeoneInGame())
			{
				game.Play();
			}

			game.StopGame();
		}
	}
}
