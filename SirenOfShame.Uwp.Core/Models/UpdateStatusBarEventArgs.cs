namespace SirenOfShame.Uwp.Core.Models
{
    public class UpdateStatusBarEventArgs {
        public const string COMMAND_NAME = "UpdateStatusBar";
        public string Exception { get; set; }
        public string StatusText { get; set; }
    }
}