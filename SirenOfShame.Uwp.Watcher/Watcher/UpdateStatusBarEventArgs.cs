using System;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class UpdateStatusBarEventArgs {
        public Exception Exception { get; set; }
        public string StatusText { get; set; }
    }
}