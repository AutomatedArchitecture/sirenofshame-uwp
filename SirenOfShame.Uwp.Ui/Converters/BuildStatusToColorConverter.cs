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

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var buildStatusEnum = (BuildStatusEnum)value;
            var color = _buildStatusToColorMap[buildStatusEnum];
            return new SolidColorBrush(color);
        }

        private static readonly Dictionary<BuildStatusEnum, Color> _buildStatusToColorMap = new Dictionary<BuildStatusEnum, Color>
        {
            { BuildStatusEnum.Working, SosColors.SuccessColor },
            { BuildStatusEnum.Broken, SosColors.FailColor },
            { BuildStatusEnum.InProgress, SosColors.PrimaryColor },
            { BuildStatusEnum.Unknown, SosColors.PrimaryColor },
        };

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}