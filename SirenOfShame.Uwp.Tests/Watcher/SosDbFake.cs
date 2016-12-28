using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Test.Unit.Watcher
{
    public class SosDbFake : SosDb
    {
        private readonly FileAdapterFake _fileAdapterFake = new FileAdapterFake();
        
        protected override FileAdapter FileAdapter => _fileAdapterFake;
    }
}