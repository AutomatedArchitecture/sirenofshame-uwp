using System.Threading.Tasks;
using Windows.UI.Xaml;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Ui.Services;

namespace SirenOfShame.Uwp.Ui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewLogsPage
    {
        public ViewLogsPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            DataContext = new ViewLogsViewModel
            {
                ShowAll = false
            };
        }

        private new ViewLogsViewModel DataContext
        {
            get { return base.DataContext as ViewLogsViewModel; }
            set { base.DataContext = value; }
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await RefreshData();
        }

        private async Task RefreshData()
        {
            var logs = await MyLogManager.ReadLogEntriesAsync(DataContext.ShowAll);
            DataContext.Events = logs.Events;
        }

        private async void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            await RefreshData();
        }
    }
}
