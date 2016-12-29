using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Test.Unit.Watcher
{
    public class SirenOfShameSettingsFake : SirenOfShameSettings {
        public SirenOfShameSettingsFake() : base(false)
        {
        }

        public override void Dirty()
        {
            // do nothing
        }

        public void DoUpgrade()
        {
            //TryUpgrade();
        }
    }
}
