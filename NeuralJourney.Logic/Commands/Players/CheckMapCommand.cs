using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.CommandContext;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Commands.Players
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.CheckMap)]
    internal class CheckMapCommand : CommandBase
    {
        private readonly int WorldWidth;
        private readonly int WorldHeight;

        internal CheckMapCommand(CommandContext commandContext, GameOptions gameOptions) : base(commandContext)
        {
            WorldWidth = gameOptions.WorldWidth;
            WorldHeight = gameOptions.WorldHeight;
        }

        internal override Task<CommandResult> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                if (Context.Player is null)
                    throw new MissingParameterException($"{Context.CommandIdentifier}", nameof(Context.Player));

                var map = new string('#', WorldWidth + 2) + "\n";
                for (var y = 0; y < WorldHeight; y++)
                {
                    map += "#";
                    for (var x = 0; x < WorldWidth; x++)
                    {
                        if (x == Context.Player.Location.X && y == Context.Player.Location.Y)
                            map += "P"; // 'P' for player
                        else
                        {
                            map += "."; // '.' for an empty room
                        }
                    }
                    map += "#\n";
                }
                map += new string('#', WorldWidth + 2) + "\n";

                return new CommandResult(map);
            });
        }
    }
}