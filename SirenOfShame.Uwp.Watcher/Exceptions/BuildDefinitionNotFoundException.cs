using System;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Exceptions
{
    public class BuildDefinitionNotFoundException : Exception
    {
        public BuildDefinitionSetting BuildDefinitionSetting { get; set; }

        public BuildDefinitionNotFoundException()
        {
        }

        public BuildDefinitionNotFoundException(BuildDefinitionSetting buildDefinitionSetting)
        {
            BuildDefinitionSetting = buildDefinitionSetting;
        }
    }
}
