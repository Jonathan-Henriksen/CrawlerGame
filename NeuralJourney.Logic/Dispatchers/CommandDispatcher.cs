using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.CommandStrategies.Interfaces;
using NeuralJourney.Logic.Dispatchers.Interfaces;

namespace NeuralJourney.Logic.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IAdminCommandStrategy _adminCommandStrategy;
        private readonly IPlayerCommandStrategy _playerCommandStrategy;

        public CommandDispatcher(IAdminCommandStrategy adminCommandStrategy, IPlayerCommandStrategy playerCommandStrategy)
        {
            _adminCommandStrategy = adminCommandStrategy;
            _playerCommandStrategy = playerCommandStrategy;
        }

        public async void DispatchAdminCommand(string adminInput)
        {
            await _adminCommandStrategy.ExecuteAsync(adminInput);
        }

        public async void DispatchPlayerCommand(string playerInput, Player player)
        {
            await _playerCommandStrategy.ExecuteAsync(playerInput, player);
        }
    }
}