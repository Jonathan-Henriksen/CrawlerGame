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

        private readonly Player _player;

        private ICommand? _playerCommand;
        private TimeOnly _time;
        private Task<string?>? _playerInputTask;

        public GameEngine(ICommandFactory commandFactory, IChatGPTService chatGPTService)
        {
            _commandFactory = commandFactory;
            _chatGPTService = chatGPTService;
            _player = new Player();
            _time = new TimeOnly(12, 0);
        }

        private bool IsRunning { get; set; }

        public IGameEngine Init()
        {
            Console.Write("Please enter you name -> ");
            var name = Console.ReadLine() ?? "Player";

            _player.SetName(name);

            Console.Write($"\n{_player.Name} -> ");

            return this;
        }

        public async void Start()
        {
            IsRunning = true;

            while (IsRunning)
            {
                await ProcessPlayerInput();

                Update();
            }
        }

        private async Task ProcessPlayerInput()
        {
            if (_playerInputTask is null)
            {
                _playerInputTask = Task.Run(Console.In.ReadLineAsync);
            }

            if (!_playerInputTask.IsCompleted)
                return;

            var playerInput = await _playerInputTask;
            _playerInputTask = null;

            if (playerInput is null)
                return;

            var response = await _chatGPTService.GetCommandFromPlayerInput(playerInput, GetAvailableCommands());
            _playerCommand = _commandFactory.GetPlayerCommand(_player, response?.Command);
        }

        private void Update()
        {
            if (_playerCommand is not null)
            {
                _playerCommand.Execute();
                _playerCommand = default;

                Console.Beep();
                Console.Write($"\n{_player.Name} -> ");
            }

            _time = _time.AddMinutes(1);
        }

        private static IEnumerable<string> GetAvailableCommands()
        {
            return Enum.GetNames(typeof(CommandEnum)).Select(d => d);
        }
    }
}