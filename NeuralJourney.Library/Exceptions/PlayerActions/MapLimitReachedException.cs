using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums.Parameters;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Library.Exceptions.PlayerActions
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