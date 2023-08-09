using Serilog.Core;
using Serilog.Events;

namespace NeuralJourney.Client
{
    public class ClientConsoleSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine(message);
        }
    }
}