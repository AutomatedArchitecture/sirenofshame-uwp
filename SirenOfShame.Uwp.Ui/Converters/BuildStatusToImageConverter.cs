using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Converters
{
    public class BuildStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var buildStatusEnum = (BuildStatusEnum)value;
            var imageName = BuildStatusToImageName[buildStatusEnum];
            var uri = new Uri($"ms-appx:///Assets/{imageName}");
            return new BitmapImage(uri);
        }

        private static readonly Dictionary<BuildStatusEnum, string> BuildStatusToImageName = new Dictionary<BuildStatusEnum, string>
        {
            { BuildStatusEnum.Working, "ok.png" },
            { BuildStatusEnum.Broken, "error.png" },
            { BuildStatusEnum.InProgress, "loadingBlue.gif" },
            { BuildStatusEnum.Unknown, "unknown.png" },
        };

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
