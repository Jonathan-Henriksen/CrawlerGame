using System.Net.Sockets;
using CrawlerGame.Library.Extensions;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Interfaces;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Services.Interfaces;

namespace CrawlerGame.Logic
{
    public class GameEngine : IGameEngine
    {
        private readonly IClockService _clock;
        private readonly ICommandFactory _commandFactory;
        private readonly IOpenAIService _openAIService;

        private readonly List<Player> _players;
        private readonly List<ICommand> _playerCommands;

        public GameEngine(IClockService clockService, ICommandFactory commandFactory, IOpenAIService openAIService)
        {
            _clock = clockService;
            _commandFactory = commandFactory;
            _openAIService = openAIService;

            _players = new List<Player>();
            _playerCommands = new List<ICommand>();
        }

        private bool IsRunning { get; set; }

        public IGameEngine Init()
        {
            return this;
        }

        public async Task StartAsync()
        {
            IsRunning = true;
            _clock.Start();

            await Task.Run(async () =>
            {
                while (IsRunning)
                {
                    await ExecutePlayerCommandsAsync();
                }
            });
        }

        public void Stop()
        {
            IsRunning = false;

            _clock.Reset();
            _playerCommands.Clear();

            foreach (var player in _players)
            {
                player.GetStream()?.Close();
            }

            _players.Clear();
        }

        public async Task HandleAdminCommandAsync(string adminInput)
        {
            await _commandFactory.GetAdminCommand(this, adminInput).ExecuteAsync();
        }

        public void AddPlayer(TcpClient playerClient)
        {
            var player = new Player(playerClient);

            _players.Add(player);

            _ = HandlePlayerAsync(player);
        }

        private async Task ExecutePlayerCommandsAsync()
        {
            var commandsToExecute = _playerCommands;
            _playerCommands.Clear();

            await Task.WhenAll(commandsToExecute.Select(command => command.ExecuteAsync()));
        }

        private async Task HandlePlayerAsync(Player player)
        {
            try
            {
                using var stream = player.GetStream();
                while (stream is not null && player.IsConnected)
                {
                    var playerInput = await GetPlayerInputAsync(stream, player);

                    if (string.IsNullOrEmpty(playerInput))
                        continue;

                    _ = stream.SendMessageAsync($"{_clock.GetTime()}: Echo -> {playerInput}");

                    //var response = await _chatGPTService.GetCommandFromPlayerInput(playerInput, GetAvailableCommands());
                    //_playerCommands.Add(_commandFactory.GetPlayerCommand(player, response?.Command));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                player.GetStream()?.Close();
                _players.Remove(player);
                Console.WriteLine($"Client disconnected: {player.Name}");
            }
        }

        private async Task<string?> GetPlayerInputAsync(NetworkStream stream, Player player)
        {
            while (player.IsConnected)
            {
                if (!stream.DataAvailable)
                    await Task.Delay(100);
                else
                    return await stream.ReadMessageAsync();
            }

            return default;
        }
    }
}
