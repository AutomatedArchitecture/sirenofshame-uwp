using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;
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
        private readonly MessageRelayService _connection;
        private int _messageNumber = 0;

        public MainPage()
        {
            InitializeComponent();
            _connection = ServiceContainer.Resolve<MessageRelayService>();
            _connection.OnMessageReceived += ConnectionOnMessageReceived;
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            var shimpty = MakePerson("Bob Shimpty");
            var gamgee = MakePerson("Sam Gamgee");
            var frodo = MakePerson("Frodo Baggins");
            var rootViewModel = new RootViewModel
            {
                BuildDefinitions = new List<BuildStatusDto>
                {
                    MakeBuildDefinition(1, "Build Def #1", BuildStatusEnum.InProgress),
                    MakeBuildDefinition(2, "Build Def #2", BuildStatusEnum.Broken),
                    MakeBuildDefinition(3, "Build Def #3", BuildStatusEnum.Unknown),
                    MakeBuildDefinition(4, "Build Def #4"),
                    MakeBuildDefinition(5, "Build Def #5"),
                    MakeBuildDefinition(6, "Build Def #6"),
                },
                Leaders = new List<PersonDto>
                {
                    new PersonDto(shimpty),
                    new PersonDto(gamgee),
                    new PersonDto(frodo)
                },
                News = new List<NewsItemDto>
                {
                    MakeNewsItem(shimpty),
                    MakeNewsItem(gamgee),
                    MakeNewsItem(frodo),
                }
            };
            DataContext = rootViewModel;
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

        private async void ConnectionOnMessageReceived(ValueSet valueSet)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
              {
                  var message = valueSet.First();
                  MyText.Text = $"{message.Key}={message.Value}";
              });
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MyText.Text = "Sending Message";
            try
            {
                await _connection.SendMessageAsync("Value", "Msg #" + _messageNumber++);
                MyText.Text = "Message Sent Successfully";
            }
            catch (Exception ex)
            {
                MyText.Text = "Send error: " + ex.Message;
            }
        }
    }
}
