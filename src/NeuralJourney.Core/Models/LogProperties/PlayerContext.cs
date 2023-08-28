namespace NeuralJourney.Core.Models.LogProperties
{
    public class PlayerContext
    {
        public string Name { get; }
        public Guid Id { get; }
        public string IpAddress { get; set; }

        public PlayerContext(string name, Guid id, string ipAddress)
        {
            Name = name;
            Id = id;
            IpAddress = ipAddress;
        }
    }
}