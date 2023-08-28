using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;
using System.Text;

namespace NeuralJourney.Core.Commands.Players.Commands
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.Map)]
    public class CheckMapCommand : CommandBase
    {
        private readonly int _worldWidth;
        private readonly int _worldHeight;

        private const char _wallChar = '#';
        private const char _floorChar = '-';
        private const char _playerChar = 'p';

        public CheckMapCommand(CommandContext context, GameOptions gameOptions) : base(context, gameOptions)
        {
            _worldWidth = gameOptions.WorldWidth;
            _worldHeight = gameOptions.WorldHeight;
        }

        public override Task<CommandResult> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                if (Context.Player is null)
                    throw new InvalidOperationException("Player was null");

                var mapBuilder = new StringBuilder("\t");
                mapBuilder.AppendLine(new string(_wallChar, _worldWidth + 2));
                for (var y = 0; y < _worldHeight; y++)
                {
                    mapBuilder.Append($"\t{_wallChar}");
                    for (var x = 0; x < _worldWidth; x++)
                    {
                        if (x == Context.Player.Location.X && y == Context.Player.Location.Y)
                            mapBuilder.Append(_playerChar);
                        else
                            mapBuilder.Append(_floorChar);
                    }
                    mapBuilder.AppendLine($"{_wallChar}");
                }
                mapBuilder.Append("\t").Append(new string(_wallChar, _worldWidth + 2));
                return new CommandResult(true, Context.ExecutionMessage, mapBuilder.ToString());
            });
        }
    }
}