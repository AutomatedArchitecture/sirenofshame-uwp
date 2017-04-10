using System;
using Windows.UI.Xaml.Data;

namespace SirenOfShame.Uwp.Ui.Converters
{
    public class ToLocalTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dateTime = value as DateTime?;
            if (value == null) return null;
            return dateTime.Value.ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
