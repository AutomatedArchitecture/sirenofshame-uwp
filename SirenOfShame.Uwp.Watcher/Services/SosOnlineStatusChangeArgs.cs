using System;

namespace SirenOfShame.Uwp.Watcher.Services
{
    public class SosOnlineStatusChangeArgs
    {
        public string TextStatus { get; set; }
        public Exception Exception { get; set; }
    }
}