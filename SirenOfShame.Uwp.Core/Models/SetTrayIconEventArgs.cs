namespace SirenOfShame.Uwp.Core.Models
{
    public class SetTrayIconEventArgs
    {
        public const string COMMAND_NAME = "SetTrayIcon";
        public TrayIcon TrayIcon { get; set; }
    }
}