using System.Threading.Tasks;
using System.Xml.Linq;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public abstract class ServiceBase
    {
        protected virtual Task<XDocument> DownloadXml(string url, string userName, string password, string cookie = null)
        {
            WebClientXml webClientXml = new WebClientXml();
            return webClientXml.DownloadXml(url, userName, password, cookie);
        }
    }
}
