using System.Threading.Tasks;

namespace SirenOfShame.Uwp.Watcher.Services
{
    public abstract class StartManagerBase
    {
        public virtual async Task Start()
        {
            RegisterServices();
            await Task.Yield();
        }

        protected abstract void RegisterServices();

        public virtual void Stop() { }
    }
}