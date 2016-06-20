using SirenOfShame.Uwp.Background.Models;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal abstract class ControllerBase
    {
        public abstract string CommandName { get; }

        public abstract SocketResult Invoke(string frame);
    }
}