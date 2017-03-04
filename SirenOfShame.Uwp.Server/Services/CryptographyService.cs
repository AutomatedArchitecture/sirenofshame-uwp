using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Services
{
    class CryptographyService : CryptographyServiceBase
    {
        public override string EncryptString(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            const BinaryStringEncoding encoding = BinaryStringEncoding.Utf8;

            // Create a buffer that contains the encoded message to be encrypted. 
            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(text, encoding);

            // Open a symmetric algorithm provider for the specified algorithm. 
            SymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(Algorithm);

            IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(DefaultKey, encoding);
            var key = objAlg.CreateSymmetricKey(keyBuffer);

            IBuffer ivBuffer = CryptographicBuffer.ConvertStringToBinary(DefaultIv, encoding);

            // Encrypt the data and return.
            IBuffer buffEncrypt = CryptographicEngine.Encrypt(key, buffMsg, ivBuffer);
            return CryptographicBuffer.EncodeToBase64String(buffEncrypt);
        }

        public override string DecryptString(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            const BinaryStringEncoding encoding = BinaryStringEncoding.Utf8;

            IBuffer buffMsg = CryptographicBuffer.DecodeFromBase64String(text);

            // Open a symmetric algorithm provider for the specified algorithm. 
            SymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(Algorithm);

            IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(DefaultKey, encoding);
            var key = objAlg.CreateSymmetricKey(keyBuffer);

            IBuffer ivBuffer = CryptographicBuffer.ConvertStringToBinary(DefaultIv, encoding);

            // Encrypt the data and return.
            IBuffer decryptedBuffer = CryptographicEngine.Decrypt(key, buffMsg, ivBuffer);
            var decryptedString = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, decryptedBuffer);
            return decryptedString;
        }
    }
}
