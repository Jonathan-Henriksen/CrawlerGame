using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Interfaces;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Services.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
            if (IsRunning || !_players.Any())
                return Task.CompletedTask;

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

        private void Stop()
        {
            IsRunning = false;

            _players.Clear();
            _playerCommands.Clear();
            _clock.Reset();
        }

        public Task HandleAdminCommandAsync(string adminInput)
        {
            return Task.Run(() =>
            {
                var command = _commandFactory.GetAdminCommand(this, adminInput);
                command.Execute();
            });
        }

        public void AddPlayer(TcpClient playerClient)
        {
            var ipEndpoint = (IPEndPoint?) playerClient.Client.RemoteEndPoint;
            var ipAddress = ipEndpoint?.Address.ToString();

            if (string.IsNullOrEmpty(ipAddress))
                return;

            var player = new Player(ipAddress);

            _players.Add(player);

            _ = StartAsync();
            _ = HandlePlayerAsync(playerClient, player);
        }

        private void ExecutePlayerCommands()
        {
            Parallel.ForEach(_playerCommands, command =>
            {
                command?.Execute();
                _playerCommands.Remove(command);
            });
        }

        private async Task HandlePlayerAsync(TcpClient client, Player player)
        {
            try
            {
                Task<string>? playerInputTask = default;

                using var stream = client.GetStream();
                while (client is not null && client.Connected)
                {
                    playerInputTask ??= GetPlayerInputAsync(stream);

                    if (!playerInputTask.IsCompleted)
                        continue;

                    var playerInput = await playerInputTask;
                    playerInputTask = default;

                    if (string.IsNullOrEmpty(playerInput))
                        continue;

                    Console.WriteLine($"{_clock.GetTime()}: {player.Name} -> {playerInput}");

                    var data = Encoding.UTF8.GetBytes($"{_clock.GetTime()}: Echo -> {playerInput}");
                    await stream.WriteAsync(data);

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
                client.Close();

                _players.Remove(player);

                if (!_players.Any())
                    Stop();

                Console.WriteLine($"Client disconnected: {player.Name}");
            }
        }

        private Task<string> GetPlayerInputAsync(NetworkStream? stream)
        {
            return Task.Run(async () =>
            {
                if (stream is null)
                    return string.Empty;

                string? inputData = default;
                while (inputData is null)
                {
                    if (!stream.DataAvailable)
                        continue;

                    var buffer = new byte[4096];

                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    inputData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }

                return inputData;
            });
        }
    }
}