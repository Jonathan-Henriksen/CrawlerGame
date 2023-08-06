namespace NeuralJourney.Logic.Commands
{
    public abstract class CommandBase
    {
        protected readonly string[]? Params;
        protected readonly string SuccessMessage;

        protected Action? CallBack;

        protected CommandBase(string[]? @params, string successMessage)
        {
            Params = @params;
            SuccessMessage = successMessage;
        }

        internal async Task ExecuteAsync()
        {
            Execute();

            await SendResponseAsync(SuccessMessage);

            CallBack?.Invoke();
        }

        protected abstract void Execute();

        protected abstract Task SendResponseAsync(string responseMessage);
    }
}