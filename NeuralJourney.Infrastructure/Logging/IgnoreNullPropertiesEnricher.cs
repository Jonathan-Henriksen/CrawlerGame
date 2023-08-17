using Serilog.Core;
using Serilog.Events;

namespace NeuralJourney.Infrastructure.Logging
{
    public class IgnoreNullPropertiesEnricher : ILogEventEnricher
    {
        private readonly HashSet<string> _propertiesToCheck;

        public IgnoreNullPropertiesEnricher(params string[] propertiesToCheck)
        {
            _propertiesToCheck = new HashSet<string>(propertiesToCheck);
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            foreach (var propertyName in _propertiesToCheck)
            {
                if (logEvent.Properties.TryGetValue(propertyName, out var propertyValue) && propertyValue is ScalarValue scalar && scalar.Value == null)
                {
                    logEvent.RemovePropertyIfPresent(propertyName);
                }
            }
        }
    }
}