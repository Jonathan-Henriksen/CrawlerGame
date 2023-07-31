using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;
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
        private readonly IOpenAIService _chatGPTService;

        private readonly List<Player> _players;
        private readonly List<Command?> _playerCommands;

        private readonly List<TcpClient> _playerConnections;

        public GameEngine(IClockService clockService, ICommandFactory commandFactory, IOpenAIService chatGPTService)
        {
            _clock = clockService;
            _commandFactory = commandFactory;
            _chatGPTService = chatGPTService;

            _players = new List<Player>();
            _playerCommands = new List<Command?>();
            _playerConnections = new List<TcpClient>();
        }

        private bool IsRunning { get; set; }

        public void AddPlayer(TcpClient playerClient)
        {
            var ipEndpoint = (IPEndPoint?) playerClient.Client.RemoteEndPoint;
            var ipAddress = ipEndpoint?.Address.ToString();

            if (string.IsNullOrEmpty(ipAddress))
                return;

            var player = new Player(ipAddress);

            _players.Add(player);
            _playerConnections.Add(playerClient);

            _ = HandlePlayerInputAsync(playerClient, player);

            Start();
        }

        public IGameEngine Init()
        {
            return this;
        }

        public void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            _clock.Start();

            while (_players.Any())
            {
                ExecutePlayerCommands();
            }

            _clock.Stop();
            IsRunning = false;
        }

        private void ExecutePlayerCommands()
        {
            foreach (var command in _playerCommands)
            {
                command?.Execute();
            }
        }

        private async Task HandlePlayerInputAsync(TcpClient client, Player player)
        {
            try
            {
                Task<string>? playerInputTask = default;

                using var stream = client.GetStream();
                while (client is not null && client.Connected)
                {
                    playerInputTask ??= Task.Run(async () =>
                    {
                        string? inputData = default;
                        while (inputData is null)
                        {
                            if (!stream.DataAvailable)
                                continue;

                            var buffer = new byte[4096];

                            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            inputData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            await stream.FlushAsync();

                            Console.WriteLine($"{player.Name} - {inputData}");

                        }

                        return inputData;
                    });

                    if (!playerInputTask.IsCompleted)
                        continue;

                    var playerInput = await playerInputTask;
                    playerInputTask = default;

                    if (string.IsNullOrEmpty(playerInput))
                        continue;

                    var data = Encoding.UTF8.GetBytes($"Echo -> {playerInput}");
                    await stream.WriteAsync(data, 0, data.Length);

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

                Console.WriteLine($"Client disconnected: {player.Name}");
            }
        }

        private static IEnumerable<string> GetAvailableCommands()
        {
            return Enum.GetNames(typeof(CommandEnum)).Select(d => d);
        }

        public void Stop()
        {
            foreach (var client in _playerConnections)
            {
                client.Close();
                _playerConnections.Remove(client);
            }

            _players.Clear();
            _playerCommands.Clear();
            _clock.Stop();
        }
    }
}