using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo.Base;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Library.Models.CommandInfo
{
    public class PlayerCommandInfo : CommandInfoBase
    {
        public PlayerCommandInfo(Player player, PlayerCommandEnum command, string[]? @params, string successMessage, string failureMessage) : base(command, @params, successMessage, failureMessage)
        {
            Player = player;
        }

        public Player Player { get; set; }
    }
}