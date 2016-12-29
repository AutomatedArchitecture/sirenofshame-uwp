using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Util;
using SirenOfShame.Uwp.Watcher.Watcher;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.Ui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainUiPage
    {
        private readonly MessageDistributorService _messageDistributorService = ServiceContainer.Resolve<MessageDistributorService>();
        private readonly MessageRelayService _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
        private RootViewModel ViewModel { get; set; }
        private readonly ILog _log = MyLogManager.GetLog(typeof(MainUiPage));
        private List<BuildStatusDto> _lastBuildStatusDtos;

        public MainUiPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            _messageDistributorService.NewNewsItem += MessageDistributorServiceOnNewNewsItem;
            _messageDistributorService.NewPerson += MessageDistributorServiceOnNewPerson;
            _messageDistributorService.RefreshStatus += MessageDistributorServiceOnRefreshStatus;
            _messageDistributorService.StatsChanged += MessageDistributorServiceOnStatsChanged;
            LoadInitialData();
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await EnsureConnected();
        }

        private async Task EnsureConnected()
        {
            var retryDelay = 10000;
            await Task.Delay(retryDelay);
            while (!_messageRelayService.IsConnected)
            {
                try
                {
                    await _messageRelayService.Open();
                }
                catch (Exception ex)
                {
                    _log.Error("Unable to connect, retrying", ex);
                    ErrorMessage.Text = $"Unable to connect to siren of shame engine. Retrying in {(retryDelay / 1000)} seconds...";
                    await Task.Delay(retryDelay);
                }
            }
        }

        private async void MessageDistributorServiceOnStatsChanged(object sender, StatsChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var leaderPairs = from oldLeader in ViewModel.Leaders
                    join newLeader in args.ChangedPeople on oldLeader.RawName equals newLeader.RawName
                    select new {oldLeader, newLeader};
                foreach (var leaderPair in leaderPairs)
                {
                    leaderPair.oldLeader.Update(leaderPair.newLeader);
                }
            });
        }

        private async void MessageDistributorServiceOnRefreshStatus(object sender, RefreshStatusEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _lastBuildStatusDtos = args.BuildStatusDtos
                    .OrderByDescending(i => i.LocalStartTime)
                    .Take(50)
                    .ToList();
                RefreshBuildStatuses(_lastBuildStatusDtos);
            });
        }

        private void RefreshBuildStatuses(List<BuildStatusDto> lastBuildStatusDtos)
        {
            RemoveAllChildControlsIfBuildCountOrBuildNamesChanged(lastBuildStatusDtos);
            if (ViewModel.BuildDefinitions.Count == 0)
            {
                ViewModel.BuildDefinitions = new ObservableCollection<BuildStatusDto>(_lastBuildStatusDtos);
            }
            else
            {
                UpdateExistingControls(_lastBuildStatusDtos);
            }
        }

        private void UpdateExistingControls(List<BuildStatusDto> lastBuildStatusDtos)
        {
            UpdateDetailsInExistingControls(lastBuildStatusDtos);
            SortExistingControls();
        }

        private void UpdateDetailsInExistingControls(List<BuildStatusDto> lastBuildStatusDtos)
        {
            var buildsJoined = from existingBd in ViewModel.BuildDefinitions
                join newBd in lastBuildStatusDtos on existingBd.BuildDefinitionId equals newBd.BuildDefinitionId
                select new {existingBd, newBd};

            foreach (var builds in buildsJoined)
            {
                builds.existingBd.Update(builds.newBd);
            }
        }

        private void SortExistingControls()
        {
            ViewModel.BuildDefinitions.Sort();
        }

        private void RemoveAllChildControlsIfBuildCountOrBuildNamesChanged(ICollection<BuildStatusDto> buildStatusDtos)
        {
            bool numberOfBuildsChanged = ViewModel.BuildDefinitions.Count != buildStatusDtos.Count();
            bool anyNewBuildDefIds = buildStatusDtos
                .Any(newBd => ViewModel.BuildDefinitions.All(oldBd => newBd.BuildDefinitionId != oldBd.BuildDefinitionId));
            _log.Debug("Removing child controls because: numberOfBuildsChanged: " + numberOfBuildsChanged);
            if (numberOfBuildsChanged || anyNewBuildDefIds)
            {
                ViewModel.BuildDefinitions.Clear();
            }
        }

        private async void MessageDistributorServiceOnNewPerson(object sender, PersonSetting personSetting)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var personDto = new PersonDto(personSetting);
                ViewModel.Leaders.Add(personDto);
            });
        }

        private async void MessageDistributorServiceOnNewNewsItem(object sender, NewNewsItemEventArgs newNewsItemEventArgs)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                NewsItemDto newsItemDto = new NewsItemDto(newNewsItemEventArgs);
                ViewModel.News.Insert(0, newsItemDto);
            });
        }

        private void LoadInitialData()
        {
            ViewModel = new RootViewModel
            {
                Leaders = new ObservableCollection<PersonDto>(),
                News = new ObservableCollection<NewsItemDto>()
            };
            DataContext = ViewModel;
        }
    }
}
