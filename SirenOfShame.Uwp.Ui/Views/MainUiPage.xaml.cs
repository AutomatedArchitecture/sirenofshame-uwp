using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Core.Util;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Ui.ViewModels;
using SirenOfShame.Uwp.Watcher.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.Ui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainUiPage
    {
        private readonly UiCommandProcessor _messageDistributorService = ServiceContainer.Resolve<UiCommandProcessor>();
        private readonly UiMessageRelayService _messageRelayService = ServiceContainer.Resolve<UiMessageRelayService>();
        private readonly NetworkService _networkService = ServiceContainer.Resolve<NetworkService>();
        private readonly GettingStartedService _gettingStartedService = ServiceContainer.Resolve<GettingStartedService>();
        private RootViewModel ViewModel { get; set; }
        private readonly ILog _log = MyLogManager.GetLog(typeof(MainUiPage));
        private List<BuildStatusDto> _lastBuildStatusDtos;
        private Timer _prettyDateTimer;

        public MainUiPage()
        {
            InitializeComponent();
            _messageDistributorService.NewNewsItem += MessageDistributorServiceOnNewNewsItem;
            _messageDistributorService.NewPerson += MessageDistributorServiceOnNewPerson;
            _messageDistributorService.RefreshStatus += MessageDistributorServiceOnRefreshStatus;
            _messageDistributorService.StatsChanged += MessageDistributorServiceOnStatsChanged;
            _messageDistributorService.UpdateStatusBar += MessageDistributorServiceOnUpdateStatusBar;
            _messageDistributorService.SetTrayIcon += MessageDistributorServiceOnSetTrayIcon;
            LoadInitialData();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_gettingStartedService.IsVeryFirstLoad)
            {
                _gettingStartedService.CompleteVeryFirstLoadAction();
                return;
            }

            // OnLoaded gets called on subsequent navigations despite NavigationCacheMode="Required"
            if (ViewModel.Initialized) return;

            try
            {
                // if we start UI and Server at the same time, give the server time to start up
                SetStatus("Connecting to build monitor...");
                await Task.Delay(2000);
                await _messageDistributorService.SendLatest();
                _prettyDateTimer = new Timer(PrettyDateOnTick, null, 0, 10000);
            }
            catch (EndpointNotFoundException)
            {
                await EnsureConnected();
                await _messageDistributorService.SendLatest();
            }
            catch (Exception ex)
            {
                await _log.Error("Error on startup", ex);
                var dialog = new MessageDialog("Error on startup: " + ex.Message);
                await dialog.ShowAsync();
            }
        }

        private async void PrettyDateOnTick(object state)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ViewModel.BuildDefinitions.ToList().ForEach(i => i.CalculatePrettyDate());
                ViewModel.News.ToList().ForEach(i => i.CalculatePrettyDate());
            });
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
                    await _log.Error("Unable to connect, retrying", ex);
                    SetStatus($"Unable to connect to siren of shame engine. Retrying in {(retryDelay / 1000)} seconds...");
                    await Task.Delay(retryDelay);
                }
            }
        }

        private void SetStatus(string statusText)
        {
            var hide = string.IsNullOrEmpty(statusText);
            Status.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
            if (statusText != null)
            {
                Status.Text = statusText;
            }
        }

        private async void MessageDistributorServiceOnSetTrayIcon(object sender, SetTrayIconEventArgs setTrayIconEventArgs)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ViewModel.TrayIcon = setTrayIconEventArgs.TrayIcon;
            });
        }

        private async void MessageDistributorServiceOnUpdateStatusBar(object sender, UpdateStatusBarEventArgs updateStatusBarEventArgs)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetStatus(updateStatusBarEventArgs.StatusText);
            });
        }

        private async void MessageDistributorServiceOnStatsChanged(object sender, StatsChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ViewModel.LeadersViewModel.StatsChanged(args.ChangedPeople);
            });
        }

        private async void MessageDistributorServiceOnRefreshStatus(object sender, RefreshStatusEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (!args.BuildStatusDtos.Any())
                {
                    var myUrl = _networkService.GetPossibleAdminPortals("; ");
                    await new MessageDialog("No builds detected.  To start watching a CI server go to the admin portal in a web browser.  Admin portal should be available at: " + myUrl).ShowAsync();
                    return;
                }

                _lastBuildStatusDtos = args.BuildStatusDtos
                    .OrderByDescending(i => i.LocalStartTime)
                    .Take(50)
                    .ToList();
                await RefreshBuildStatuses(_lastBuildStatusDtos);
            });
        }

        private async Task RefreshBuildStatuses(List<BuildStatusDto> lastBuildStatusDtos)
        {
            await RemoveAllChildControlsIfBuildCountOrBuildNamesChanged(lastBuildStatusDtos);
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

        private async Task RemoveAllChildControlsIfBuildCountOrBuildNamesChanged(ICollection<BuildStatusDto> buildStatusDtos)
        {
            bool numberOfBuildsChanged = ViewModel.BuildDefinitions.Count != buildStatusDtos.Count;
            bool anyNewBuildDefIds = buildStatusDtos
                .Any(newBd => ViewModel.BuildDefinitions.All(oldBd => newBd.BuildDefinitionId != oldBd.BuildDefinitionId));
            if (numberOfBuildsChanged || anyNewBuildDefIds)
            {
                await _log.Debug("Removing child controls because: numberOfBuildsChanged: " + numberOfBuildsChanged);
                ViewModel.BuildDefinitions.Clear();
            }
        }

        private async void MessageDistributorServiceOnNewPerson(object sender, NewUserEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ViewModel.LeadersViewModel.AddPerson(args.NewPeople);
            });
        }

        private async void MessageDistributorServiceOnNewNewsItem(object sender, NewNewsItemEventArgs newNewsItemEventArgs)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var newsItemEvent in newNewsItemEventArgs.NewsItemEvents)
                {
                    if (newsItemEvent.ShouldUpdateOldInProgressNewsItem)
                        TryToFindAndUpdateOldInProgressNewsItem(newsItemEvent);
                    else
                        AddNewsItemToPanel(newsItemEvent);
                }
            });
        }

        private void AddNewsItemToPanel(NewsItemEvent newsItemEvent)
        {
            NewsItemDto newsItemDto = new NewsItemDto(newsItemEvent);
            ViewModel.News.Insert(0, newsItemDto);
        }

        private void TryToFindAndUpdateOldInProgressNewsItem(NewsItemEvent newsItemEvent)
        {
            var oldBuild = ViewModel.News.FirstOrDefault(i => i.BuildId == newsItemEvent.BuildId);
            if (oldBuild == null)
            {
                AddNewsItemToPanel(newsItemEvent);
            }
            else
            {
                oldBuild.UpdateState(newsItemEvent);
            }
        }

        private void LoadInitialData()
        {
            ViewModel = new RootViewModel
            {
                LeadersViewModel = new LeadersViewModel
                {
                    Leaders = new ObservableCollection<PersonDto>(),
                },
                News = new ObservableCollection<NewsItemDto>()
            };
            DataContext = ViewModel;
        }

        private async void RefreshOnTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                ViewModel.Clear();
                await _messageDistributorService.SendLatest();
            }
            catch (EndpointNotFoundException ex)
            {
                await _log.Error("Endpoint not found after tapping refresh", ex);
                var dialog = new MessageDialog("Error refreshing status, can't find server.  Maybe try again shortly.");
                await dialog.ShowAsync();
            }
        }
    }
}
