namespace SirenOfShame.Uwp.Background.Controllers
{
    internal abstract class ApiController
    {
        public abstract string Get(HttpContext context);
    }
}