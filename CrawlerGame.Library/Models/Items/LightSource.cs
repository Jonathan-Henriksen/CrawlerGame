using CrawlerGame.Library.Models.Items.Base;
using Timer = System.Timers.Timer;

namespace CrawlerGame.Library.Models.Items
{
    internal class LightSource : ItemBase
    {
        internal LightSource(string name, string? activationPhrase = null, string? activationMessage = null, int? initialUses = null, int? usageDuration = null) : base(name, activationPhrase, activationMessage, initialUses)
        {
            IsActivated = false;

            if (!usageDuration.HasValue)
                return;

            UsageTimer = new Timer(usageDuration.Value)
            {
                AutoReset = false
            };

            UsageTimer.Elapsed += (sender, e) => Deactivate();
        }

        private readonly Timer? UsageTimer;

        private bool IsActivated { get; set; }

        public void Activate()
        {
            if (RemainingUses == 0)
                return;

            if (UsageTimer is null)
            {
                IsActivated = true;
                return;
            }

            IsActivated = true;
            UsageTimer.Start();
        }

        public void Deactivate()
        {
            if (!IsActivated)
                return;

            IsActivated = false;
        }
    }
}