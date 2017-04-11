namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public enum TrayIcon
    {
        Red,
        Green,
        Question
    }
    
    public class SetTrayIconEventArgs
    {
        public const string COMMAND_NAME = "SetTrayIcon";
        public TrayIcon TrayIcon { get; set; }
    }
}