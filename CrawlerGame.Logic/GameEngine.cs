using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Interfaces;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Services.Interfaces;

namespace CrawlerGame.Logic
{
    public class GameEngine : IGameEngine
    {
        private readonly ICommandFactory _commandFactory;
        private readonly IChatGPTService _chatGPTService;

        private ICommand _playerCommand;
        private readonly Player _player;

        public GameEngine(ICommandFactory commandFactory, IChatGPTService chatGPTService)
        {
            _commandFactory = commandFactory;
            _chatGPTService = chatGPTService;
            _player = new Player();
            _playerCommand = _commandFactory.GetPlayerCommand(_player, null);
        }

        private bool GameIsRunning { get; set; }

        public void Start()
        {
            GameIsRunning = true;
        }

        public void Stop()
        {
            GameIsRunning = false;
        }

        public bool IsRunning()
        {
            return GameIsRunning;
        }

        public void SetPlayerName(string playerName)
        {
            _player.SetName(playerName);
        }

        public string GetPlayerInput()
        {
            Console.Write($"\n{_player.Name} -> ");
            return Console.ReadLine() ?? string.Empty;
        }

        public async void Update(string userInput)
        {
            var response = await _chatGPTService.GetCommandFromPlayerInput(userInput, GetAvailableCommands());

            _playerCommand = _commandFactory.GetPlayerCommand(_player, response?.Command);
            _playerCommand.Execute();
        }

        private IEnumerable<string> GetAvailableCommands()
        {
            return Enum.GetNames(typeof(Direction)).Select(d => d);
        }
    }
}