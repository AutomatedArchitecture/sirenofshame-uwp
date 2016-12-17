using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Converters
{
    public class BuildStatusToColorConverter : IValueConverter
    {
        private static readonly Color SuccessColor = Color.FromArgb(255, 81, 163, 81);
        private static readonly Color FailColor = Color.FromArgb(255, 189, 54, 47);
        private static readonly Color PrimaryColor = Color.FromArgb(255, 40, 95, 152);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var buildStatusEnum = (BuildStatusEnum)value;
            var color = BuildStatusToColorMap[buildStatusEnum];
            return new SolidColorBrush(color);
        }

        private static readonly Dictionary<BuildStatusEnum, Color> BuildStatusToColorMap = new Dictionary<BuildStatusEnum, Color>
        {
            { BuildStatusEnum.Working, SuccessColor },
            { BuildStatusEnum.Broken, FailColor },
            { BuildStatusEnum.InProgress, PrimaryColor },
            { BuildStatusEnum.Unknown, PrimaryColor },
        };

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}