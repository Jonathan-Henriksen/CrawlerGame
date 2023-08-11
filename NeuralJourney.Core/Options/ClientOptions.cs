namespace NeuralJourney.Core.Options
{
    public sealed class ClientOptions
    {
        public string ServerIp { get; set; } = "localhost";

        public int ServerPort { get; set; }
    }
}