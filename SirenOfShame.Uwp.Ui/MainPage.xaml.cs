using System;
using System.Linq;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.Ui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            App._connection.OnMessageReceived += ConnectionOnOnMessageReceived;
        }

        private async void ConnectionOnOnMessageReceived(ValueSet valueSet)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
              {
                  var message = valueSet.First();
                  MyText.Text = $"{message.Key}={message.Value}";
              });
        }

        private int _messageNumber = 0;

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MyText.Text = "Sending Message";
            try
            {
                await App._connection.SendMessageAsync("Value", "Msg #" + _messageNumber++);
                MyText.Text = "Message Sent Successfully";
            }
            catch (Exception ex)
            {
                MyText.Text = "Send error: " + ex.Message;
            }
        }
    }
}
