using CrawlerGame.Library.Models.ChatGPT;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;

namespace CrawlerGame.Logic.Factories.Interfaces
{
    public interface ICommandFactory
    {
        internal Command GetPlayerCommand(Player player, CommandInfo commandInfo);

        internal Command GetAdminCommand(IGameEngine game, string adminInput, params string[] parameters);
    }
}