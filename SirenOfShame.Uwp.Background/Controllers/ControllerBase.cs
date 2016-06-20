using System.Threading.Tasks;
using SirenOfShame.Uwp.Background.Models;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal abstract class ControllerBase
    {
        public abstract string CommandName { get; }

        public abstract Task<SocketResult> Invoke(string frame);
    }
}