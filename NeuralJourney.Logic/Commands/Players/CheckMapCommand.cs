using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Extensions;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Commands.Players
{
    [PlayerCommand(PlayerCommandEnum.CheckMap)]
    internal class CheckMapCommand : PlayerCommand
    {
        private readonly Player Player;

        private readonly int WorldWidth;
        private readonly int WorldHeight;

        internal CheckMapCommand(PlayerCommandInfo commandInfo, GameOptions gameOptions) : base(commandInfo)
        {
            Player = commandInfo.Player;
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