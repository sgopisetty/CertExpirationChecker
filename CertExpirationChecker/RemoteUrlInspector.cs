using CertExpirationChecker;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace WebsitesDashboard.Util
{
    public class RemoteUrlInspector
    {
        private string siteName;
        private Uri Url;

        public async Task GetExpiration(string siteName, string url)
        {

            this.siteName = siteName;
            try
            {
                this.Url = new Uri(url);
            }
            catch
            {
                var msg = "Not a valid Url.. skipping : " + url;
                Logger.LogMessage(msg);
            }


            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = CertCallBack
            };

            try
            {
                HttpClient cl = new HttpClient(handler);
                var x = await cl.GetAsync(url);
            }
            catch
            {

            }

        }
        private bool CertCallBack(HttpRequestMessage arg1, X509Certificate2? arg2, X509Chain? arg3, SslPolicyErrors arg4)
        {
            foreach (X509Extension extension in arg2.Extensions)
            {
                // Create an AsnEncodedData object using the extensions information.
                AsnEncodedData asndata = new AsnEncodedData(extension.Oid, extension.RawData);
                if (extension.Oid.FriendlyName.Equals("Subject Alternative Name"))
                {
                    //Console.WriteLine(asndata.Format(true));
                    var allSANs = new List<string>(asndata.Format(true).Replace("\n", "").Split('\r'));
                    if (allSANs.Contains("DNS Name=" + Url.Host))
                    {
                        var msg = $"Site: {siteName}, DNS: {Url.Host}, Expiration: {arg2.NotAfter}, Issuer: {arg2.Issuer}";
                        Logger.LogMessage(msg);
                    }
                }
            }

            return arg2.NotAfter <= DateTime.Now && DateTime.Now <= arg2.NotAfter;
        }
    }
}
