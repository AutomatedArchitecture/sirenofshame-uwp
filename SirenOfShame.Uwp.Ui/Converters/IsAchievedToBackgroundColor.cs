using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SirenOfShame.Uwp.Ui.Converters
{
    public class IsAchievedToBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isAchieved = (bool) value;
            var color = isAchieved ? Color.FromArgb(255, 50, 175, 82) : Colors.DimGray;
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IsAchievedToForegroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isAchieved = (bool) value;
            var color = isAchieved ? Colors.White : Colors.Silver;
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
