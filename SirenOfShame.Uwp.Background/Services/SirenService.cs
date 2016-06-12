using SirenOfShame.Device;

namespace SirenOfShame.Uwp.Background.Services
{
    internal class SirenService
    {
        public static SirenService Instance = new SirenService();
        private SirenOfShameDevice _device;

        private SirenService()
        {
            _device = new SirenOfShameDevice();
            _device.StartWatching();
        }

        public SirenOfShameDevice Device => _device;
    }
}
