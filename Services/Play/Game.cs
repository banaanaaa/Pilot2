using Pilot2.Models;
using Pilot2.Services.Chat;
using Pilot2.Services.Storage;
using Pilot2.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

		public int InputNumberOfPlayers()
		{
			int count;
			while (true)
			{
				WriteLine("Input the number of players");
				var input = ReadLine(Formating.Number);

				try
				{
					if (input.StartsWith("/"))
					{
						ExecuteOnCommand(input);
					}
					else
					{
						var tmp = Convert.ToInt32(input);
						if (tmp >= _minPlayersCount)
						{
							count = tmp;
							break;
						}
						else
						{
							throw new ArgumentException("Need at least 2 players to start game");
						}
					}
				}
				catch (Exception ex)
				{
					WriteLineError(ex.Message);
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
					WriteLine($"Enter name for the {i + 1} player");
					var input = ReadLine(Formating.Line);

					try
					{
						if (input.StartsWith("/"))
						{
							ExecuteOnCommand(input);
						}
						else
						{
							if (!PlayerIsExist(input))
							{
								success = true;
								AddPlayer(new Player()
								{
									Id = Guid.NewGuid(),
									Name = input,
									Score = 0,
									Words = new Collection<string>(),
									inGame = true
								});
								break;
							}
							else
							{
								throw new ArgumentException("Name already taken. Try again");
							}
						}
					}
					catch (Exception ex)
					{
						WriteLineError(ex.Message);
					}
				}
			}
			return success;
		}

		public void AddPlayer(Player player)
		{
			if (!GameActive)
			{
				_players.Add(player);
			}
			else
			{
				throw new ArgumentException("Game already started, cannot add new player");
			}
		}

		public void Play()
		{
			try
			{
				if (!GameActive)
				{
					throw new ArgumentException("Game not started yet");
				}

				foreach (var player in _players)
				{
					if (player.inGame)
					{
						var isSuccess = HandleRound(player);
						if (isSuccess)
						{
							player.Score++;
						}
						else if (SomeoneInGame())
						{
							player.inGame = false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				WriteLineError(ex.Message);
			}
		}

		public void StartGame()
		{
			try
			{
				GameActive = GameActive ? throw new ArgumentException("Game already started") : true;
				if (_players.Count < _minPlayersCount)
				{
					throw new ArgumentException("need at least 2 players to start game");
				}
				BaseWord = InputBaseWord(_minBasewordLength, _maxBasewordLength);
			}
			catch (Exception ex)
			{
				WriteLineError(ex.Message);
			}
		}

		public async void StopGame()
		{
			try
			{
				GameActive = !GameActive ? throw new ArgumentException("Game not started yet") : false;

				await _storageService.AddGameResultsAsync(_players.Select(t => new GameResultItem() { UserId = t.Id, UserName = t.Name, Score = t.Score }));

				WriteLineInfo($"{FindWinner()} win!");
			}
			catch (Exception ex)
			{
				WriteLineError(ex.Message);
			}
		}

		private void Score()
		{
			try
			{
				if (_players.Count == 0)
				{
					throw new ArgumentException("No players");
				}

				WriteLineInfo("Total score");
				foreach (var pl in _players)
				{
					WriteLineInfo($"{pl.Name}\t-> {pl.Score + _storageService.GetScore(pl.Name)}");
				}
			}
			catch (Exception ex)
			{
				WriteLineError(ex.Message);
			}
		}

		private void ShowWords()
		{
			try
			{
				if (_players.Count == 0)
				{
					throw new ArgumentException("No players");
				}
				foreach (var pl in _players)
				{
					WriteLineInfo($"Words by {pl.Name}:");
					if (pl.Words.Count == 0)
					{
						WriteLineInfo("\tnothing");
					}
					else
					{
						foreach (var word in pl.Words)
						{
							WriteLineInfo($"\t{word}");
						}
					}
				}
			}
			catch (ArgumentException ex)
			{
				WriteLineError(ex.Message);
			}
		}

		private void TotalScore()
		{
			try
			{
				var playersResultsInGame = (_players.Count != 0) ?
					_players.Select(t => new SortedCollection() { Name = t.Name, Score = t.Score}) :
					new Collection<SortedCollection>();
				var playersResultsInData = _storageService.GetLoadedData();

				IEnumerable<SortedCollection> list = new Collection<SortedCollection>();
				if (playersResultsInGame.Count() != 0 && playersResultsInData != null)
				{
					list = playersResultsInData.Select(t => new SortedCollection { Name = t.UserName, Score = t.Score }).Union(playersResultsInGame);
				}
				else if (playersResultsInData != null)
				{
					list = playersResultsInData.Select(t => new SortedCollection { Name = t.UserName, Score = t.Score });
				}
				else
				{
					list = playersResultsInGame;
				}

				WriteLineInfo("Total score");
				if (list.Count() == 0)
				{
					WriteLineInfo("nothing");
					throw new ArgumentException("Data is empty or no players");
				}
				else
				{
					//TODO
					foreach (var res in list.Distinct().OrderByDescending(t => t.Score))
					{
						WriteLineInfo($"{res.Name}\t-> {res.Score}");
					}
				}
			}
			catch (Exception ex)
			{
				WriteLineError(ex.Message);
			}
		}

		private bool HandleRound(Player player)
		{
			bool success = false;
			for (int attempt = 0; attempt < _maxPlayerAttempts; attempt++)
			{
				WriteLine($"{player.Name} [{attempt + 1}/{_maxPlayerAttempts}]");
				var input = ReadLine(Formating.Line);

				try
                {
					if (input.StartsWith("/"))
					{
						ExecuteOnCommand(input);
						attempt--;
					}
					else if (BaseWordContainsAllChars(input))
					{
						player.Words.Add(input);
						success = true;
						break;
					}
					else
					{
						throw new ArgumentException("Failed. Try again");
					}
				}
				catch (Exception ex)
				{
					if (ex.Message == "No such command")
					{
						attempt--;
					}
					WriteLineError(ex.Message);
					if (attempt + 1 == _maxPlayerAttempts)
                    {
						player.inGame = false;
						WriteLineInfo($"{player.Name} lost");
					}
				}
			}
			return success;
		}

		private void ExecuteOnCommand(string input)
		{
			if (_commands.ContainsKey(input))
			{
				_commands[input]();
			}
			else
			{
				throw new ArgumentException("No such command");
			}
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
				WriteLine($"Enter base word with length >= {minLength} & <= {maxLength}");
				var input = ReadLine(Formating.Line);
				
				try
                {

					if (input.StartsWith("/"))
					{
						ExecuteOnCommand(input);
					}
					else if (input.Length < minLength || input.Length > maxLength)
                    {
						throw new ArgumentException("Wrong length. Try again");
					}
					else
					{
						word = input;
					}
				}
				catch (Exception ex)
				{
					WriteLineError(ex.Message);
				}
			}
			return word;
		}

		public bool SomeoneInGame() => _players.Any(t => t.inGame);
		private string FindWinner() => _players.OrderByDescending(t => t.Score).First().Name;
		private bool BaseWordContainsAllChars(string compare) => compare.All(t => BaseWord.Contains(t));
	}
}
