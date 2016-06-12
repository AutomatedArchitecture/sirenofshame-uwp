using System.Threading.Tasks;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal abstract class ApiController
    {
        public abstract Task<string> Get(HttpContext context);
    }
}