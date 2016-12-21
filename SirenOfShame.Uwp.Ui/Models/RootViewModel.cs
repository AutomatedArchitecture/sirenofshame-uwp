using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SirenOfShame.Uwp.Ui.Annotations;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Models
{
    public class RootViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BuildStatusDto> _buildDefinitions = new ObservableCollection<BuildStatusDto>();

        public ObservableCollection<BuildStatusDto> BuildDefinitions
        {
            get { return _buildDefinitions; }
            set
            {
                if (Equals(value, _buildDefinitions)) return;
                _buildDefinitions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NewsItemDto> News { get; set; }
        public ObservableCollection<PersonDto> Leaders { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
