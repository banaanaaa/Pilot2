﻿using Pilot2.Models;
using Pilot2.Services.Chat;
using Pilot2.Services.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Pilot2.Services.Play
{
	class Game : NewConsole, IGame
	{
		public bool GameActive { get; set; }

		private string BaseWord;

		private const int _minPlayersCount = 2;

		private const int _minBasewordLength = 8;

		private const int _maxBasewordLength = 32;

		private const int _maxPlayerAttempts = 3;

		private readonly ICollection<Player> _players;

		private readonly Dictionary<string, Action> _commands;

		private readonly IStorageService _storageService;

		public Game(IStorageService storageService)
		{
			_storageService = storageService;
			GameActive = false;

			_commands = new Dictionary<string, Action>
			{
				{ "/score", Score },
				{ "/show-words", ShowWords },
				{ "/total-score", TotalScore }
			};

			_players = new Collection<Player>();
		}

		public void Score()
		{

		}

		public void ShowWords()
		{

		}

		public void TotalScore()
		{

		}

		private bool HandleRound(Player player)
		{
			bool success = false;
			for (int attempt = 0; attempt < _maxPlayerAttempts; attempt++)
			{
				WriteLine($"[{attempt + 1}/{_maxPlayerAttempts}]");
				var input = ReadLine(Formating.Line);

				if (input.StartsWith("/"))
					ExecuteOnCommand(input);
				else if (BaseWordContainsAllChars(input))
				{
					player.Words.Add(input);
					success = true;
					break;
				}
				else
					throw new ArgumentException("failed.try again");
			}
			return success;
		}

		private void ExecuteOnCommand(string input)
		{
			if (_commands.ContainsKey(input))
				_commands[input]();
			else
				throw new ArgumentException("No such command");
		}

		public void AddPlayer(Player player)
		{
			if (!GameActive)
				_players.Add(player);
			else
				throw new ArgumentException("Game already started, cannot add new player");
		}

		public void ExecuteRound()
		{
			if (!GameActive)
				throw new ArgumentException("Game not started yet");

			foreach (var player in _players)
			{
				if (player.inGame)
				{
					var isSuccess = HandleRound(player);
					if (isSuccess)
						player.Score++;
					// if someone inGame - continue
					else if (_players.Any(t => t.inGame))
						player.inGame = false;
					// if all !inGame - stop game
					else
						StopGame();
				}
			}
		}

		public void StartGame()
		{
			GameActive = GameActive ? throw new ArgumentException("Game already started") : true;

			if (_players.Count < _minPlayersCount)
				throw new ArgumentException("need at least 2 players to start game");

			BaseWord = InputBaseWord(_minBasewordLength, _maxBasewordLength);
		}

		public async void StopGame()
		{
			GameActive = !GameActive ? throw new ArgumentException("Game not started yet") : false;

			await _storageService.AddGameResultsAsync(_players.Select(t => new GameResultItem() { UserId = t.Id, Name = t.Name, Score = t.Score }));
		}

		public int InputNumberOfPlayers()
		{
			int count = 0;
			while (true)
			{
				WriteLine("input the number of players");
				var input = ReadLine(Formating.Number);

				if (input.StartsWith("/"))
					ExecuteOnCommand(input);
				else
				{
					var tmp = Convert.ToInt32(input);
					if (tmp >= _minPlayersCount)
					{
						count = tmp;
						break;
					}
					else
						throw new ArgumentException("need at least 2 players to start game");
				}
			}
			return count;
		}

		public bool InitPlayers(int count)
		{
			bool success = false;
			for (int i = 0; i < count; i++)
			{
				while (true)
				{
					WriteLine($"enter name for the {i + 1} player");
					var input = ReadLine(Formating.Line);

					if (input.StartsWith("/"))
						ExecuteOnCommand(input);
					else
					{
						if (!PlayerIsExist(input))
						{
							success = true;
							AddPlayer(new Player() {
								Id = Guid.NewGuid(),
								Name = input,
								Score = 0,
								Words = new Collection<string>(),
								inGame = true
							});
							break;
						}
						else
							throw new ArgumentException("player is exist. try again");
					}
				}
			}
			return success;
		}

		public bool PlayerIsExist(string compare)
		{
			bool success = false;
			foreach  (var player in _players)
			{
				if (player.Name == compare)
				{
					success = true;
					break;
				}
			}
			return success;
		}

		public string InputBaseWord(int minLength, int maxLength)
		{
			string word = "";
			while (word.Length == 0)
			{
				WriteLine($"enter base word with length >= {minLength} & <= {maxLength}");
				var input = ReadLine(Formating.Line);
				if (input.StartsWith("/"))
					ExecuteOnCommand(input);
				else if (input.Length < minLength || input.Length > maxLength)
					throw new ArgumentException("wrong length");
				else
					word = input;
			}
			return word;
		}

		public bool BaseWordContainsAllChars(string compare) => compare.All(t => BaseWord.Contains(t));
	}
}
