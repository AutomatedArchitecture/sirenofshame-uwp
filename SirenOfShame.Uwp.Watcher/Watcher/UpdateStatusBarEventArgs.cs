namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class UpdateStatusBarEventArgs {
        public const string COMMAND_NAME = "UpdateStatusBar";
        public string Exception { get; set; }
        public string StatusText { get; set; }
    }
}