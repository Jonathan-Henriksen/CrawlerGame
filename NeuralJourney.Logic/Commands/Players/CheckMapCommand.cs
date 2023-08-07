using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Extensions;
using NeuralJourney.Library.Models.CommandContext;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Commands.Players
{
    [CommandIdentifier(CommandIdentifierEnum.CheckMap)]
    internal class CheckMapCommand : PlayerCommandBase
    {
        private readonly int WorldWidth;
        private readonly int WorldHeight;

        internal CheckMapCommand(CommandContext commandContext, GameOptions gameOptions) : base(commandContext)
        {
            WorldWidth = gameOptions.WorldWidth;
            WorldHeight = gameOptions.WorldHeight;
        }

        protected override void Execute()
        {
            var map = new string('#', WorldWidth + 2) + "\n";
            for (var y = 0; y < WorldHeight; y++)
            {
                map += "#";
                for (var x = 0; x < WorldWidth; x++)
                {
                    if (x == Player.Location.X && y == Player.Location.Y)
                        map += "P"; // 'P' for player
                    else
                    {
                        map += "."; // '.' for an empty room
                    }
                }
                map += "#\n";
            }
            map += new string('#', WorldWidth + 2) + "\n";

            CallBack = async () => { await Player.GetStream().SendMessageAsync(map); };
        }
    }
}