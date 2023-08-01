using System.Net.Sockets;
using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Extensions;
using CrawlerGame.Library.Models.Player;
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

        public GameEngine(IClockService clockService, ICommandFactory commandFactory, IOpenAIService openAIService)
        {
            _clock = clockService;
            _commandFactory = commandFactory;
            _openAIService = openAIService;
            _players = new List<Player>();
        }

        private bool IsRunning { get; set; }

        public IGameEngine Init()
        {
            return this;
        }

        public void Start()
        {
            IsRunning = true;
            _clock.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            _clock.Reset();

            foreach (var player in _players)
            {
                player.GetStream()?.Close();
            }

            _players.Clear();
        }

        public async Task ExecuteAdminCommandAsync(string input)
        {
            await _commandFactory.GetAdminCommand(this, input).ExecuteAsync();
        }

        public void AddPlayer(TcpClient playerClient)
        {
            var player = new Player(playerClient);

            _players.Add(player);

            _ = HandlePlayerInputAsync(player);
        }

        private async Task HandlePlayerInputAsync(Player player)
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

                    //var commandInfo = await _openAIService.GetCommandFromPlayerInput(playerInput, Enum.GetNames(typeof(CommandEnum)));

                    //await _commandFactory.GetPlayerCommand(player, commandInfo).ExecuteAsync();
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
