using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Services.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

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

            _ = StartAsync();

            _ = HandlePlayerInputAsync(playerClient, player);
        }

        public IGameEngine Init()
        {
            return this;
        }

        public Task StartAsync()
        {
            return Task.Run(() =>
            {
                if (IsRunning || !_players.Any())
                    return;

                IsRunning = true;
                _clock.Start();

                while (IsRunning)
                {
                    ExecutePlayerCommands();
                }

                _clock.Stop();
                IsRunning = false;
            });
        }

        public void Stop()
        {
            IsRunning = false;

            foreach (var client in _playerConnections)
            {
                client.Close();
                _playerConnections.Remove(client);
            }

            _players.Clear();
            _playerCommands.Clear();
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

        private static IEnumerable<string> GetAvailableCommands()
        {
            return Enum.GetNames(typeof(CommandEnum)).Select(d => d);
        }
    }
}