using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Exceptions.PlayerActions.Base;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Library.Exceptions.PlayerActions
{
    [Serializable]
    public class MapLimitReachedException : PlayerActionException
    {
        public MapLimitReachedException() { }
        public MapLimitReachedException(string message) : base(message) { }
        public MapLimitReachedException(string message, Exception inner) : base(message, inner) { }
        protected MapLimitReachedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public MapLimitReachedException(Player player, DirectionEnum? direction) :
            base(string.Format("You cannot move any further {0]", direction), player)
        {

        }
    }
}