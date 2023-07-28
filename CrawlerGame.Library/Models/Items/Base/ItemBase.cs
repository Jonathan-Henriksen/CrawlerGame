namespace CrawlerGame.Library.Models.Items.Base
{
    internal class ItemBase
    {
        public ItemBase(string name, string? activationPhrase, string? activationMessage, int? initialUses)
        {
            Name = name;
            ActivationPhrase = activationPhrase ?? $"Activate {Name}";
            ActivationMessage = activationMessage ?? $"Activated {Name}";
            RemainingUses = initialUses ?? -1;
        }

        public string Name { get; set; }

        public string ActivationPhrase { get; set; }

        public string ActivationMessage { get; set; }

        public int RemainingUses { get; set; }
    }
}