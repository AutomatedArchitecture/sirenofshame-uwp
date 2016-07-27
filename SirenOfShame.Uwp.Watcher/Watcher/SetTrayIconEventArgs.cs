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
        public TrayIcon TrayIcon { get; set; }
    }
}