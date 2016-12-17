using System.Collections.Generic;
using System.Collections.ObjectModel;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Models
{
    public class RootViewModel
    {
        public List<BuildStatusDto> BuildDefinitions { get; set; }
        public ObservableCollection<NewsItemDto> News { get; set; }
        public List<PersonDto> Leaders { get; set; }
    }
}
