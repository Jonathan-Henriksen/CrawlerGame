using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Services.Interfaces;

namespace CrawlerGame.Logic
{
    public class GameEngine : IGameEngine
    {
        private readonly IClockService _clock;
        private readonly ICommandFactory _commandFactory;
        private readonly IOpenAIService _chatGPTService;

        private readonly Player _player;

        private Command? _playerCommand;
        private Task<string?>? _playerInputTask;

        public GameEngine(IClockService clockService, ICommandFactory commandFactory, IOpenAIService chatGPTService)
        {
            _clock = clockService;
            _commandFactory = commandFactory;
            _chatGPTService = chatGPTService;
            _player = new Player();
        }

        private bool IsRunning { get; set; }

        private bool IsPaused { get; set; }

        public IGameEngine Init()
        {
            Console.Write("Please enter you name -> ");
            var name = Console.ReadLine() ?? "Player";

            _player.SetName(name);
            _clock.Start();

            Console.Write($"\n{_player.Name} -> ");

            return this;
        }

        public async void Start()
        {
            IsRunning = true;
            IsPaused = false;

            while (IsRunning)
            {
                await ProcessPlayerInput();

                Update();
            }
        }

        public void TogglePause()
        {
            if (IsPaused)
                _clock.Resume();
            else
                _clock.Pause();

            IsPaused = !IsPaused;
        }

        private async Task ProcessPlayerInput()
        {
            _playerInputTask ??= Task.Run(Console.In.ReadLineAsync);

            if (!_playerInputTask.IsCompleted)
                return;

            var playerInput = await _playerInputTask;

            if (playerInput is null)
                return;

            var response = await _chatGPTService.GetCommandFromPlayerInput(playerInput, GetAvailableCommands());
            _playerCommand = _commandFactory.GetPlayerCommand(_player, response?.Command);
        }

        private void Update()
        {
            ExecutePlayerCommand();
        }

        private static IEnumerable<string> GetAvailableCommands()
        {
            return Enum.GetNames(typeof(CommandEnum)).Select(d => d);
        }

        private void ExecutePlayerCommand()
        {
            if (_playerCommand is null)
                return;

            _playerCommand.Execute();
            _playerCommand = default;

            Console.Beep();
            Console.Write($"\n{_player.Name} -> ");
        }
    }
}