namespace NeuralJourney.Core.Models.LogProperties
{
    public class MessageContext
    {
        public MessageContext(string messageText, int retryCount, string ipAddress)
        {
            MessageText = messageText;
            RetryCount = retryCount;
            IpAddress = ipAddress;
        }

        public string MessageText { get; set; }

        public int RetryCount { get; set; }

        public string IpAddress { get; set; }
    }
}