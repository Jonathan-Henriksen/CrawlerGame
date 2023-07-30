using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Options;
using CrawlerGame.Logic.Services.Interfaces;
using System.Net;
using System.Net.Sockets;

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

                Task<string?>? playerInputTask = default;

                while (client.Connected)
                {
                    using var stream = client.GetStream();
                    using var reader = new StreamReader(stream);

                    playerInputTask ??= Task.Run(reader.ReadLineAsync);

                    if (!playerInputTask.IsCompleted)
                        continue;

                    var playerInput = await playerInputTask;

                    if (playerInput is null)
                        continue;

                    //var response = await _chatGPTService.GetCommandFromPlayerInput(playerInput, GetAvailableCommands());

                    //_playerCommands.Add(_commandFactory.GetPlayerCommand(player, response?.Command));

                    Console.WriteLine($"{player.Name} - {playerInput}");

                    using var writer = new StreamWriter(stream) { AutoFlush = true };

                    _ = writer.WriteAsync($"Echo: {playerInput}");
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