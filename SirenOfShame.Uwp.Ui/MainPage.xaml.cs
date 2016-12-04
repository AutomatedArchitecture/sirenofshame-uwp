using System;
using System.Linq;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using SirenOfShame.Uwp.Ui.Services;

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
