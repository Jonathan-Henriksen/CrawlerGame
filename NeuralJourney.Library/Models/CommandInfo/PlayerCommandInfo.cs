using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo.Base;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Library.Models.CommandInfo
{
    public class PlayerCommandInfo : CommandInfoBase<PlayerCommandEnum>
    {
        public PlayerCommandInfo(PlayerCommandEnum commandNameEnum, string[]? @params, string successMessage, Player player) : base(commandNameEnum, @params, successMessage)
        {
            Player = player;
        }

        public Player Player { get; set; }
    }
}