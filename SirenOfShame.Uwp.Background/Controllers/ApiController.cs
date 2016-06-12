using System.Threading.Tasks;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal abstract class ApiController
    {
        public abstract Task<object> Get(HttpContext context);

        public virtual async Task Post(HttpContext context)
        {
            await Task.Yield();
        }
    }
}