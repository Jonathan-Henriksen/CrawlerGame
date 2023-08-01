using CrawlerGame.Library.Extensions;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Interfaces;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Services.Interfaces;
using System.Net.Sockets;

namespace CrawlerGame.Logic
{
    public class GameEngine : IGameEngine
    {
        private readonly IClockService _clock;
        private readonly ICommandFactory _commandFactory;
        private readonly IOpenAIService _openAIService;

        private readonly List<Player> _players;
        private readonly List<ICommand?> _playerCommands;

        public GameEngine(IClockService clockService, ICommandFactory commandFactory, IOpenAIService openAIService)
        {
            _clock = clockService;
            _commandFactory = commandFactory;
            _openAIService = openAIService;

            _players = new List<Player>();
            _playerCommands = new List<ICommand?>();
        }

        private bool IsRunning { get; set; }

        public IGameEngine Init()
        {
            return this;
        }

        public Task StartAsync()
        {
            IsRunning = true;
            _clock.Start();

            return Task.Run(() =>
            {
                while (IsRunning)
                {
                    ExecutePlayerCommands();
                }
            });
        }

        public Task HandleAdminCommandAsync(string adminInput)
        {
            return Task.Run(() =>
            {
                _commandFactory.GetAdminCommand(this, adminInput).Execute();
            });
        }

        public void AddPlayer(TcpClient playerClient)
        {
            var player = new Player(playerClient);

            _players.Add(player);

            _ = HandlePlayerAsync(player);
        }

        private void ExecutePlayerCommands()
        {
            Parallel.ForEach(_playerCommands, command =>
            {
                command?.Execute();
                _playerCommands.Remove(command);
            });
        }

        private async Task HandlePlayerAsync(Player player)
        {
            try
            {
                Task<string?>? playerInputTask = default;

                using var stream = player.GetStream();
                while (player.IsConnected)
                {
                    playerInputTask ??= GetPlayerInputAsync(stream, player);

                    if (!playerInputTask.IsCompleted)
                        continue;

                    var playerInput = await playerInputTask;
                    playerInputTask = default;

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
                player.GetStream().Close();

                _players.Remove(player);

                Console.WriteLine($"Client disconnected: {player.Name}");
            }
        }

        private Task<string?> GetPlayerInputAsync(NetworkStream stream, Player player)
        {
            return Task.Run(async () =>
            {
                while (player.IsConnected)
                {
                    if (!stream.DataAvailable)
                        continue;

                    return await stream.ReadMessageAsync();
                }

                return default;
            });
        }

        public void Stop()
        {
            IsRunning = false;

            _clock.Reset();
            _playerCommands.Clear();

            foreach (var player in _players)
            {
                player.GetStream().Close();
                _players.Remove(player);
            }
        }
    }
}