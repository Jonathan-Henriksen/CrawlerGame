using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Library.Exceptions.PlayerActions.Base
{
    [Serializable]
    public class PlayerActionException : Exception
    {
        public readonly Player? Player;

        public PlayerActionException() { }
        public PlayerActionException(string message) : base(message) { }
        public PlayerActionException(string message, Exception inner) : base(message, inner) { }
        protected PlayerActionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public PlayerActionException(string message, Player? player) : base(message)
        {
            Player = player;
        }
    }
}