using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Converters
{
    public class AvatarIdToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var avatarId = ((int?)value) ?? 0;
            // avatarId's are zero based, but resources are 1 based
            avatarId++;
            if (avatarId <= 0) avatarId = 1;
            var uri = new Uri($"ms-appx:///Assets/Avatars/Avatar{avatarId:00}.png");
            return new BitmapImage(uri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}