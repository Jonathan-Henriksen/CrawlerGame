namespace NeuralJourney.Core.Models.Options
{
    public sealed class ClientOptions
    {
        public string ServerIp { get; set; } = "localhost";

        public int ServerPort { get; set; }
    }
}