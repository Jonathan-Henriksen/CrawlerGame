using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Extensions;
using NeuralJourney.Library.Models.ChatGPT;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Base;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Commands.Gameplay
{
    [CommandMapping(CommandEnum.CheckMap)]
    internal class CheckMapCommand : Command
    {
        private readonly Player? _player;
        private readonly int _worldWidth;
        private readonly int _worldHeight;


        public CheckMapCommand(CommandInfo commandInfo, GameOptions gameOptions) : base(commandInfo)
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
                for (int y = 0; y < _worldHeight; y++)
                {
                    map += "#";
                    for (int x = 0; x < _worldWidth; x++)
                    {
                        if (x == _player?.Location?.X && y == _player?.Location?.Y)
                        {
                            map += "P"; // 'P' for player
                        }
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