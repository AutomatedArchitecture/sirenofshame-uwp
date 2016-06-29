using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirenOfShame.Uwp.Watcher.Settings
{
    public class TripleDesStringEncryptor
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        //private readonly TripleDESCryptoServiceProvider _provider;

        public TripleDesStringEncryptor()
        {
            //_key = Encoding.ASCII.GetBytes("GSYAHAGCBDUUADIADKOPAAAW");
            //_iv = Encoding.ASCII.GetBytes("USAZBGAW");
            //_provider = new TripleDESCryptoServiceProvider();
        }

        public string EncryptString(string text)
        {
            // todo: figure out encryption/decrption
            return text;

            //ICryptoTransform transform = _provider.CreateEncryptor(_key, _iv);

            //if (string.IsNullOrWhiteSpace(text))
            //{
            //    return null;
            //}
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            //    {
            //        byte[] input = Encoding.UTF8.GetBytes(text);
            //        cryptoStream.Write(input, 0, input.Length);
            //        cryptoStream.FlushFinalBlock();
            //        return Convert.ToBase64String(stream.ToArray());
            //    }
            //}
        }

        public string DecryptString(string text)
        {
            // todo: figure out encryption/decryption
            return text;

            //ICryptoTransform transform = _provider.CreateDecryptor(_key, _iv);

            //if (string.IsNullOrWhiteSpace(text))
            //{
            //    return null;
            //}
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            //    {
            //        byte[] input = Convert.FromBase64String(text);
            //        cryptoStream.Write(input, 0, input.Length);
            //        cryptoStream.FlushFinalBlock();
            //        return Encoding.UTF8.GetString(stream.ToArray());
            //    }
            //}
        }
    }
}
