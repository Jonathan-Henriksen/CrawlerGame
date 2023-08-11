using NeuralJourney.Core.Constants.Messages;
using NeuralJourney.Core.Enums.Parameters;
using NeuralJourney.Core.Models.World;

namespace NeuralJourney.Core.Exceptions.World
{
    [Serializable]
    public class MapLimitReachedException : GameException
    {
        public readonly Player Player;

        public readonly DirectionEnum Direction;

        public MapLimitReachedException(Player player, DirectionEnum direction) :
            base(PlayerMessageTemplates.Map.LimitReached, ErrorMessageTemplates.Map.LimitReached, direction, player.Name)
        {
            Player = player;
            Direction = direction;
        }
    }
}