namespace NeuralJourney.Core.Models.LogProperties
{
    public class PlayerContext
    {
        public PlayerContext(string name, Guid id, string ipAddress)
        {
            Name = name;
            Id = id;
            IpAddress = ipAddress;
        }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public string IpAddress { get; set; }
    }
}