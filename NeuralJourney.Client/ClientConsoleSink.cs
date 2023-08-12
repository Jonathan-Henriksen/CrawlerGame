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

            Console.Write("> ");
            WriteColoredMessage(message, ConsoleColor.Blue);
            Console.Write("> ");
        }

        private void WriteColoredMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{message}\n");
            Console.ResetColor();
        }
    }
}