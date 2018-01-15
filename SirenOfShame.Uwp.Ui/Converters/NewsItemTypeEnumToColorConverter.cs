using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using SirenOfShame.Uwp.Core.Models;

namespace SirenOfShame.Uwp.Ui.Converters
{
    public class NewsItemTypeEnumToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var buildStatusEnum = (NewsItemTypeEnum)value;
            var color = GetColorForEventType(_newsItemToColorMap, buildStatusEnum, SosColors.PrimaryColor);
            return new SolidColorBrush(color);
        }

        private static Color GetColorForEventType(Dictionary<NewsItemTypeEnum, Color> dictionary, NewsItemTypeEnum newsItemEventType, Color defaultColor)
        {
            Color color;
            if (dictionary.TryGetValue(newsItemEventType, out color))
                return color;
            return defaultColor;
        }

        private static readonly Dictionary<NewsItemTypeEnum, Color> _newsItemToColorMap = new Dictionary<NewsItemTypeEnum, Color>
        {
            { NewsItemTypeEnum.BuildSuccess, SosColors.SuccessColor },
            { NewsItemTypeEnum.BuildStarted, SosColors.PrimaryColor },
            { NewsItemTypeEnum.SosOnlineComment, Color.FromArgb(255, 0, 102, 221) },
            { NewsItemTypeEnum.SosOnlineMisc, Color.FromArgb(255, 73, 175, 205) },
            { NewsItemTypeEnum.SosOnlineNewAchievement, SosColors.AchievementColor },
            { NewsItemTypeEnum.SosOnlineNewMember, Color.FromArgb(255, 73, 175, 205) },
            { NewsItemTypeEnum.SosOnlineReputationChange, Color.FromArgb(255, 73, 175, 205) },
            { NewsItemTypeEnum.BuildFailed, SosColors.FailColor },
            { NewsItemTypeEnum.NewAchievement, SosColors.AchievementColor },
        };

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}