using System.Collections.Generic;

namespace SirenOfShame.Uwp.Core.Models
{
    public class NewUserEventArgs
    {
        public const string COMMAND_NAME = "NewUser";
        public List<PersonSettingBase> NewPeople { get; set; }
    }
}