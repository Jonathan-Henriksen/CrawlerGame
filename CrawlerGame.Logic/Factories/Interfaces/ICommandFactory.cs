using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;

namespace CrawlerGame.Logic.Factories.Interfaces
{
    public interface ICommandFactory
    {
        internal Command GetPlayerCommand(Player player, CommandEnum? command);
    }
}