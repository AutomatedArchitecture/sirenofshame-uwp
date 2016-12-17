using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Models
{
    public class RootViewModel
    {
        public List<BuildStatusDto> BuildDefinitions { get; set; }
        public List<NewNewsItemEventArgs> News { get; set; }
        public List<PersonSetting> Leaders { get; set; }
    }
}
