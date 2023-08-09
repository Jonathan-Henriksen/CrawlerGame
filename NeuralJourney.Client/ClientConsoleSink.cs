using Serilog.Core;
using Serilog.Events;

namespace NeuralJourney.Client
{
    public class ClientConsoleSink : ILogEventSink
    {
        //private readonly IFormatProvider _formatProvider;

        //public ClientConsoleSink(IFormatProvider formatProvider)
        //{
        //    _formatProvider = formatProvider;
        //}


        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine(message);
        }
    }
}