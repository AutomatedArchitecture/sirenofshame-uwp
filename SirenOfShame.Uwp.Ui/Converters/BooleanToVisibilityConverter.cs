using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SirenOfShame.Uwp.Ui.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool Inverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (Inverted)
            {
                return true.Equals(value) ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return true.Equals(value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
