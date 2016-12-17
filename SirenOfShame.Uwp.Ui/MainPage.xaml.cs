using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.Ui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly MessageAggregatorService _messageAggregatorService = ServiceContainer.Resolve<MessageAggregatorService>();
        private readonly MessageRelayService _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
        private RootViewModel ViewModel { get; set; }
        private readonly ILog _log = MyLogManager.GetLog(typeof(MainPage));

        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            _messageAggregatorService.NewNewsItem += MessageAggregatorServiceOnNewNewsItem;
            _messageAggregatorService.NewPerson += MessageAggregatorServiceOnNewPerson;
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

        private async void MessageAggregatorServiceOnNewPerson(object sender, PersonSetting personSetting)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var personDto = new PersonDto(personSetting);
                ViewModel.Leaders.Add(personDto);
            });
        }

        private async void MessageAggregatorServiceOnNewNewsItem(object sender, NewNewsItemEventArgs newNewsItemEventArgs)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                NewsItemDto newsItemDto = new NewsItemDto(newNewsItemEventArgs);
                ViewModel.News.Add(newsItemDto);
            });
        }

        private void LoadInitialData()
        {
            var shimpty = MakePerson("Bob Shimpty");
            var gamgee = MakePerson("Sam Gamgee");
            var frodo = MakePerson("Frodo Baggins");
            ViewModel = new RootViewModel
            {
                BuildDefinitions = new ObservableCollection<BuildStatusDto>
                {
                    MakeBuildDefinition(1, "Build Def #1", BuildStatusEnum.InProgress),
                    MakeBuildDefinition(2, "Build Def #2", BuildStatusEnum.Broken),
                    MakeBuildDefinition(3, "Build Def #3", BuildStatusEnum.Unknown),
                    MakeBuildDefinition(4, "Build Def #4"),
                    MakeBuildDefinition(5, "Build Def #5"),
                    MakeBuildDefinition(6, "Build Def #6"),
                },
                Leaders = new ObservableCollection<PersonDto>
                {
                    new PersonDto(shimpty),
                    new PersonDto(gamgee),
                    new PersonDto(frodo)
                },
                News = new ObservableCollection<NewsItemDto>
                {
                    MakeNewsItem(shimpty),
                    MakeNewsItem(gamgee),
                    MakeNewsItem(frodo),
                }
            };
            DataContext = ViewModel;
        }

        private NewsItemDto MakeNewsItem(PersonSetting person)
        {
            var newNewsItemEventArgs = new NewNewsItemEventArgs
            {
                Title = "Achieved Shame Pusher",
                EventDate = DateTime.Now.AddHours(-1),
                Person = person,
                ReputationChange = 1
            };
            return new NewsItemDto(newNewsItemEventArgs);
        }

        private PersonSetting MakePerson(string displayName)
        {
            return new PersonSetting
            {
                RawName = displayName,
                DisplayName = displayName,
                TotalBuilds = 200,
                FailedBuilds = 20,
                CurrentBuildRatio = 2,
                CurrentSuccessInARow = 2,
                NumberOfTimesFixedSomeoneElsesBuild = 1
            };
        }

        private static BuildStatusDto MakeBuildDefinition(int id, string title, BuildStatusEnum buildStatusEnum = BuildStatusEnum.Working)
        {
            return new BuildStatusDto
            {
                BuildDefinitionId = id.ToString(),
                BuildDefinitionDisplayName = title,
                BuildStatusEnum = buildStatusEnum,
                RequestedByDisplayName = "Lee Richardson",
                LocalStartTime = DateTime.Now,
                Comment = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                Duration = "1:15"
            };
        }
    }
}
