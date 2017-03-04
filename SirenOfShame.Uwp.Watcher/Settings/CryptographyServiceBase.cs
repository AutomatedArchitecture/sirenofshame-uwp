namespace SirenOfShame.Uwp.Watcher.Settings
{
    public abstract class CryptographyServiceBase
    {
        public abstract string EncryptString(string text);
        public abstract string DecryptString(string text);
    }
}
