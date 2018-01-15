using System.Collections.Generic;

namespace SirenOfShame.Uwp.Core.Models
{
    public class RefreshStatusEventArgs {
        public const string COMMAND_NAME = "RefreshStatus";
        public IList<BuildStatusDto> BuildStatusDtos { get; set; }

        //public void RefreshDisplayNames(SirenOfShameSettings settings)
        //{
        //    foreach (var buildStatusDto in BuildStatusDtos)
        //    {
        //        buildStatusDto.SetDisplayName(settings);
        //    }
        //}
    }
}