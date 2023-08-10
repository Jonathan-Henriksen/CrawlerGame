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

            if (Console.GetCursorPosition().Top > 0)
            {
                Console.WriteLine();
            }

            Console.Write("> ");
            WriteColoredMessage(message, ConsoleColor.Blue);
            Console.Write("\n> ");
        }

        private void WriteColoredMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{message}");
            Console.ResetColor();
        }
    }
}