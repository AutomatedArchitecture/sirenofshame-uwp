namespace SirenOfShame.Uwp.Watcher.Settings
{
    public abstract class CryptographyServiceBase
    {
        protected const string DefaultKey = "GSYAHAGCBDUUADIADKOPAAAW";
        protected const string DefaultIv = "USAZBGAW";
        protected const string Algorithm = "3DES_CBC_PKCS7";

        public abstract string EncryptString(string text);
        public abstract string DecryptString(string text);
    }
}
