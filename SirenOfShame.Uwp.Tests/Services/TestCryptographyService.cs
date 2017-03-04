using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Tests.Services
{
    public class TestCryptographyService : CryptographyServiceBase
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        private readonly TripleDESCryptoServiceProvider _provider;

        public TestCryptographyService()
        {
            _key = Encoding.ASCII.GetBytes("GSYAHAGCBDUUADIADKOPAAAW");
            _iv = Encoding.ASCII.GetBytes("USAZBGAW");
            _provider = new TripleDESCryptoServiceProvider();
        }

        public override string EncryptString(string text)
        {
            ICryptoTransform transform = _provider.CreateEncryptor(_key, _iv);

            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                {
                    byte[] input = Encoding.UTF8.GetBytes(text);
                    cryptoStream.Write(input, 0, input.Length);
                    cryptoStream.FlushFinalBlock();
                    return Convert.ToBase64String(stream.ToArray());
                }
            }
        }

        public override string DecryptString(string text)
        {
            ICryptoTransform transform = _provider.CreateDecryptor(_key, _iv);

            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                {
                    byte[] input = Convert.FromBase64String(text);
                    cryptoStream.Write(input, 0, input.Length);
                    cryptoStream.FlushFinalBlock();
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }
    }
}
