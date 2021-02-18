using Pilot2.Models;

namespace Pilot2.Services.Play
{
	interface IGame
	{
        bool GameActive { get; set; }
        int InputNumberOfPlayers();
        bool InitPlayers(int count);
        void AddPlayer(Player player);
        void Play();
        void StartGame();
        void StopGame();
        bool SomeoneInGame();
    }
}
