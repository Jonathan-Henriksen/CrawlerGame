using System.Net.Sockets;
using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Extensions;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;

namespace NeuralJourney.Logic
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
            _openAIService.Init();

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
                _players.Remove(player);
            }
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
                var stream = player.GetStream();
                while (player.IsConnected)
                {
                    var playerInput = await GetPlayerInputAsync(stream, player);

                    if (string.IsNullOrEmpty(playerInput))
                        continue;

                    var commandInfo = await _openAIService.GetCommandFromPlayerInput(playerInput, Enum.GetNames(typeof(CommandEnum)));

                    Console.WriteLine($"{player.Name} -> {playerInput} -> Command:{commandInfo.Command} | Message:{commandInfo.SuccessMessage}");
                    _ = stream.SendMessageAsync($"{_clock.GetTime()}: {commandInfo.SuccessMessage}");

                    //await _commandFactory.GetPlayerCommand(player, commandInfo).ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Phrases.System.ClientError}: {ex.Message}");
            }
            finally
            {
                player.GetStream()?.Close();
                _players.Remove(player);
                Console.WriteLine($"{Phrases.System.ClientConnected}: {player.Name}");
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
