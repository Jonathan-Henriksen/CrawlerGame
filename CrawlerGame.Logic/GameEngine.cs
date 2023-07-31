using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Options;
using CrawlerGame.Logic.Services.Interfaces;
using System;
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
        private readonly TcpListener _server;

        private readonly List<Player> _players;

        private readonly List<Command?> _playerCommands;


        public GameEngine(IClockService clockService, ICommandFactory commandFactory, IOpenAIService chatGPTService, ServerOptions serverOptions)
        {
            _clock = clockService;
            _commandFactory = commandFactory;
            _chatGPTService = chatGPTService;

            _server = new TcpListener(IPAddress.Any, serverOptions.Port);
            _players = new List<Player>();
            _playerCommands = new List<Command?>();
        }

        private bool IsRunning { get; set; }

        private bool IsPaused { get; set; }

        public IGameEngine Init()
        {
            _ = StartServer();

            return this;
        }

        public void Start()
        {
            IsRunning = true;
            IsPaused = false;

            _clock.Start();

            while (IsRunning)
            {
                ExecutePlayerCommands();
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

        private void Update()
        {
            ExecutePlayerCommands();
        }

        private void ExecutePlayerCommands()
        {
            foreach (var command in _playerCommands)
            {
                command?.Execute();
                Console.Beep();
            }
        }

        private Task StartServer()
        {
            _server.Start();

            return Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("Starting server and waiting for clients");
                    while (IsRunning)
                    {
                        var client = await _server.AcceptTcpClientAsync();
                        var ip = (IPEndPoint?) client.Client.RemoteEndPoint;

                        var player = new Player(ip?.Address.ToString() ?? string.Empty);

                        _players.Add(player);
                        _ = HandleClientAsync(client, player);
                    }
                }
                finally
                {
                    _server.Stop();
                }
            });
        }

        private async Task HandleClientAsync(TcpClient client, Player player)
        {
            try
            {
                Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

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

                            byte[] buffer = new byte[4096];

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

                    if (playerInput is null)
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
                Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}");
            }
        }

        private static IEnumerable<string> GetAvailableCommands()
        {
            return Enum.GetNames(typeof(CommandEnum)).Select(d => d);
        }
    }
}