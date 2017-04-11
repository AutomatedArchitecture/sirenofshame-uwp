using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class NewUserEventArgs
    {
        public const string COMMAND_NAME = "NewUser";
        public List<PersonSetting> NewPeople { get; set; }
    }
}