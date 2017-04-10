using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MetroLog.Targets;
using SirenOfShame.Uwp.Ui.Annotations;

namespace SirenOfShame.Uwp.Ui.Models
{
    public sealed class ViewLogsViewModel : INotifyPropertyChanged
    {
        private List<LogEventInfoItem> _events;
        private bool _showAll;

        public List<LogEventInfoItem> Events
        {
            get { return _events; }
            set
            {
                if (Equals(value, _events)) return;
                _events = value;
                OnPropertyChanged();
            }
        }

        public bool ShowAll
        {
            get { return _showAll; }
            set
            {
                if (value == _showAll) return;
                _showAll = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}