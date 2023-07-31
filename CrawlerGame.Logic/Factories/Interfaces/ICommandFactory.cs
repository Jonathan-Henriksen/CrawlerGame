using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Interfaces;

namespace CrawlerGame.Logic.Factories.Interfaces
{
    public interface ICommandFactory
    {
        internal ICommand GetPlayerCommand(Player player, CommandEnum? command);

        internal ICommand GetAdminCommand(IGameEngine game, string adminInput, params string[] parameters);
    }
}