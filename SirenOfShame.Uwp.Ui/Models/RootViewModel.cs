using System.Collections.ObjectModel;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Models
{
    public class RootViewModel
    {
        public ObservableCollection<BuildStatusDto> BuildDefinitions { get; set; }
        public ObservableCollection<NewsItemDto> News { get; set; }
        public ObservableCollection<PersonDto> Leaders { get; set; }
    }
}
