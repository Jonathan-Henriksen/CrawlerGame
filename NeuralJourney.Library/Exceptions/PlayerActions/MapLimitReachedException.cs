using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums.Parameters;
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

        public MapLimitReachedException(Player? player, DirectionEnum? direction) :
            base(string.Format(ErrorMessages.PlayerActions.MapLimitReached, direction), player)
        {

        }
    }
}