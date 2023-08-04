using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Extensions;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Commands.Players
{
    [PlayerCommandMapping(PlayerCommandEnum.CheckMap)]
    internal class CheckMapCommand : PlayerCommand
    {
        private readonly Player? _player;
        private readonly int _worldWidth;
        private readonly int _worldHeight;


        public CheckMapCommand(PlayerCommandInfo commandInfo, GameOptions gameOptions) : base(commandInfo)
        {
            _player = commandInfo.Player;
            _worldWidth = gameOptions.WorldWidth;
            _worldHeight = gameOptions.WorldHeight;
        }

        protected override (bool Success, Action? Callback) Execute()
        {
            try
            {
                var map = new string('#', _worldWidth + 2) + "\n";
                for (var y = 0; y < _worldHeight; y++)
                {
                    map += "#";
                    for (var x = 0; x < _worldWidth; x++)
                    {
                        if (x == _player?.Location?.X && y == _player?.Location?.Y)
                            map += "P"; // 'P' for player
                        else
                        {
                            map += "."; // '.' for an empty room
                        }
                    }
                    map += "#\n";
                }
                map += new string('#', _worldWidth + 2) + "\n";

                var callback = () => { _player?.GetStream().SendMessageAsync(map); };

                return (true, callback);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);

                return (false, null);
            }
        }
    }
}