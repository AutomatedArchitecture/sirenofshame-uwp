using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using MetroLog.Targets;
using SirenOfShame.Uwp.Ui.Annotations;
using SirenOfShame.Uwp.Ui.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SirenOfShame.Uwp.Ui.Views
{
    public class ViewLogsDataContext : INotifyPropertyChanged
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewLogs
    {
        public ViewLogs()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            DataContext = new ViewLogsDataContext
            {
                ShowAll = false
            };
        }

        private new ViewLogsDataContext DataContext
        {
            get { return base.DataContext as ViewLogsDataContext; }
            set { base.DataContext = value; }
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await RefreshData();
        }

        private async Task RefreshData()
        {
            var logs = await MetroLogger.ReadLogEntriesAsync(DataContext.ShowAll);
            DataContext.Events = logs.Events;
        }

        private async void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            await RefreshData();
        }
    }
}
