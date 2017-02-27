using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Tests.Services
{
    public class FakeSettingsIoService : SettingsIoService
    {
        public override async Task DeleteSettings()
        {
            await Task.Yield();
        }

        public override Task<SirenOfShameSettings> GetFromDiskOrDefault()
        {
            return Task.FromResult(GetDefaultSettings());
        }

        public override async Task Save()
        {
            await Task.Yield();
        }

        public override async Task SaveIfDirty()
        {
            await Task.Yield();
        }
    }
}
