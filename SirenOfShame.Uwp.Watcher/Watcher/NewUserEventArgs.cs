using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class NewUserEventArgs
    {
        public List<PersonSetting> NewPeople { get; set; }
    }
}