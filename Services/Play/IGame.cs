using Pilot2.Models;

namespace Pilot2.Services.Play
{
	interface IGame
	{
        bool GameActive { get; set; }

        bool BaseWordContainsAllChars(string compare);
        string InputBaseWord(int minLength, int maxLength);
        int InputNumberOfPlayers();
        bool InitPlayers(int count);
        bool PlayerIsExist(string compare);
        void AddPlayer(Player player);
        void Play();
        void StartGame();
        void StopGame();
    }
}
