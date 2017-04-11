using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Converters
{
    public class TrayIconToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var buildStatusEnum = (TrayIcon)value;
            var imageName = _trayIconToImageName[buildStatusEnum];
            var uri = new Uri($"ms-appx:///Assets/{imageName}");
            return new BitmapImage(uri);
        }

        private static readonly Dictionary<TrayIcon, string> _trayIconToImageName = new Dictionary<TrayIcon, string>
        {
            { TrayIcon.Green, "ok.png" },
            { TrayIcon.Red, "error.png" },
            { TrayIcon.Question, "unknown.png" },
        };
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
