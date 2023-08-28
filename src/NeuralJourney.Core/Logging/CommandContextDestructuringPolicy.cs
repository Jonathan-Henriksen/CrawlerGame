using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.LogProperties;
using Serilog.Core;
using Serilog.Events;

namespace NeuralJourney.Core.Logging
{
    public class CommandContextDestructuringPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            if (value is CommandContext context)
            {
                var commandKey = new { context.CommandKey.Type, context.CommandKey.Identifier };

                var properties = new List<LogEventProperty>
                {
                    new LogEventProperty(nameof(CommandKey), propertyValueFactory.CreatePropertyValue(commandKey, true)),
                    new LogEventProperty(nameof(context.InputText), new ScalarValue(context.InputText))
                };

                if (context.Params?.Any() ?? false)
                {
                    var @params = context.Params.Select(item => propertyValueFactory.CreatePropertyValue(item, true)).ToArray();
                    var paramsProperty = new SequenceValue(@params);

                    properties.Add(new LogEventProperty(nameof(context.Params), paramsProperty));
                }

                if (context.CompletionText is not null)
                    properties.Add(new LogEventProperty(nameof(context.CompletionText), new ScalarValue(context.CompletionText)));

                if (context.ExecutionMessage is not null)
                    properties.Add(new LogEventProperty(nameof(context.ExecutionMessage), new ScalarValue(context.ExecutionMessage)));

                if (context.Result is not null && context.Result.HasValue && context.Result.Value.AdditionalMessage is not null)
                    properties.Add(new LogEventProperty(nameof(context.Result.Value.AdditionalMessage), new ScalarValue(context.Result.Value.AdditionalMessage)));

                var structure = new StructureValue(properties);

                result = structure;
                return true;
            }

            result = new StructureValue(Enumerable.Empty<LogEventProperty>());
            return false;
        }
    }
}