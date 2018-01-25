using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace SirenOfShame.Uwp.Maintenance.Services
{
    /// <summary>
    /// Provides an HttpClient that will disable man-in-the-middle attacks via certificate pinning based 
    /// on a known public key, certificate common name, and base url.
    /// </summary>
    internal sealed class CertificatePinningHttpClientFactory
    {
        private readonly string _baseUrl;
        private readonly string _certificateCommonName;
        private readonly string _certificatePublicKey;

        // todo: move this into a configuration file

        public CertificatePinningHttpClientFactory(string baseUrl, string certificateCommonName, string certificatePublicKey)
        {
            _baseUrl = baseUrl;
            _certificatePublicKey = certificatePublicKey;
            _certificateCommonName = certificateCommonName;
        }

        public async Task<T> WithHttpClient<T>(Func<HttpClient, Task<T>> func)
        {
            using (var filter = new HttpBaseProtocolFilter())
            {
                filter.ServerCustomValidationRequested += FilterOnServerCustomValidationRequested;
                var httpClient = new HttpClient(filter);
                var result = await func(httpClient);
                filter.ServerCustomValidationRequested -= FilterOnServerCustomValidationRequested;
                return result;
            }
        }

        public async Task WithHttpClient(Func<HttpClient, Task> action)
        {
            await WithHttpClient(async httpClient =>
            {
                await action(httpClient);
                return true;
            });
        }

        private void FilterOnServerCustomValidationRequested(HttpBaseProtocolFilter sender, HttpServerCustomValidationRequestedEventArgs args)
        {
            if (!IsCertificateValid(args.RequestMessage, args.ServerCertificate, args.ServerCertificateErrors))
            {
                args.Reject();
            }
        }

        private bool IsCertificateValid(Windows.Web.Http.HttpRequestMessage httpRequestMessage, Certificate cert, IReadOnlyList<ChainValidationResult> sslPolicyErrors)
        {
            // disallow self-signed certificates or certificates with errors
            if (sslPolicyErrors.Count > 0)
            {
                return false;
            }

            if (RequestRequiresCheck(httpRequestMessage.RequestUri))
            {
                var certificateSubject = cert?.Subject;
                bool subjectMatches = certificateSubject == _certificateCommonName;

                var certArray = cert?.GetCertificateBlob().ToArray();
                var x509Certificate2 = new X509Certificate2(certArray);
                var certificatePublicKey = x509Certificate2.GetPublicKey();
                var certificatePublicKeyString = Convert.ToBase64String(certificatePublicKey);
                bool publicKeyMatches = certificatePublicKeyString == _certificatePublicKey;

                return subjectMatches && publicKeyMatches;
            }

            // by default reject any requests that don't match up to our known base url
            return false;
        }

        private bool RequestRequiresCheck(Uri uri)
        {
            return uri.IsAbsoluteUri &&
                   uri.AbsoluteUri.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase) &&
                   uri.AbsoluteUri.StartsWith(_baseUrl, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
