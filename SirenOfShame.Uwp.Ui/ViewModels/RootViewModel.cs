using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Ui.Annotations;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.ViewModels
{
    public class RootViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BuildStatusDto> _buildDefinitions = new ObservableCollection<BuildStatusDto>();
        private TrayIcon? _trayIcon;

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
        public LeadersViewModel LeadersViewModel { get; set; }

        public TrayIcon? TrayIcon
        {
            get { return _trayIcon; }
            set
            {
                if (value == _trayIcon) return;
                _trayIcon = value;
                OnPropertyChanged();
            }
        }

        public bool Initialized => BuildDefinitions != null && BuildDefinitions.Any();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Clear()
        {
            LeadersViewModel.Leaders.Clear();
            News.Clear();
            BuildDefinitions.Clear();
        }
    }
}
