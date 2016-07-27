using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class TrayNotifyEventArgs {
        public string Title { get; set; }

        public string TipText { get; set; }

        public SosToolTipIcon TipIcon { get; set; }
    }
}