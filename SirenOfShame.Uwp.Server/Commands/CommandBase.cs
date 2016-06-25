using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal abstract class CommandBase
    {
        public abstract string CommandName { get; }

        public abstract Task<SocketResult> Invoke(string frame);
    }
}