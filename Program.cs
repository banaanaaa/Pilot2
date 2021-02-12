﻿using Pilot2.Services.Play;
using Pilot2.Services.Storage;
using Pilot2.Services.Chat;

namespace Pilot2
{
	class Program
	{
		static void Main(string[] args)
		{
			StorageService storageService = new StorageService("../../gamelog.json");
			Game game = new Game(storageService);
			NewConsole Console = new NewConsole();

			int count = game.InputNumberOfPlayers();

			game.InitPlayers(count);

			game.StartGame();

			game.ExecuteRound();
		}
	}
}