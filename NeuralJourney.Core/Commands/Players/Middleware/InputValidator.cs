﻿using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Exceptions.Commands;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Commands.Players.Middleware
{
    public class InputValidation : ICommandMiddleware
    {
        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(context.RawInput))
                throw new InvalidCommandException(CommandIdentifierEnum.Unknown, "Input was blank");

            await next();
        }
    }
}