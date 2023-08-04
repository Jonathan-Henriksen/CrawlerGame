namespace NeuralJourney.Logic.Commands
{
    public abstract class CommandBase
    {
        protected readonly string[]? Params;
        protected readonly string SuccessMessage;
        protected readonly string FailureMessage;

        protected CommandBase(string[]? @params, string successMessage, string failureMessage)
        {
            Params = @params;
            SuccessMessage = successMessage;
            FailureMessage = failureMessage;
        }

        internal async Task ExecuteAsync()
        {
            var (success, callback) = Execute();

            var responseMessage = success ? SuccessMessage : FailureMessage;

            await SendResponseAsync(responseMessage);

            callback?.Invoke();
        }

        protected abstract (bool Success, Action? Callback) Execute();

        protected abstract Task SendResponseAsync(string responseMessage);
    }
}