namespace SirenOfShame.Uwp.Web.Controllers
{
    internal abstract class ControllerBase
    {
        public abstract string CommandName { get; }

        public abstract SocketResult Invoke(string frame);
    }
}