using System.Globalization;
using SirenOfShame.Uwp.Ui.Helpers;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Models
{
    public class NewsItemDto
    {
        public string Title { get; }
        public string DisplayName { get; }
        public string Project { get; }
        public string ReputationChange { get; }
        public string When { get; set; }
        public NewsItemTypeEnum NewsItemTypeEnum { get; set; }

        public NewsItemDto(NewNewsItemEventArgs news)
        {
            ReputationChange = news.ReputationChange.HasValue ? GetNumericAsDelta(news.ReputationChange.Value) : null;
            Title = GetTitle(news);
            DisplayName = news.Person.DisplayName;
            Project = news.BuildDefinitionId?.ToUpperInvariant();
            When = news.EventDate.PrettyDate();
            NewsItemTypeEnum = news.NewsItemType;
        }

        private static string GetTitle(NewNewsItemEventArgs news)
        {
            return news.NewsItemType == NewsItemTypeEnum.SosOnlineComment ? "\"" + news.Title + "\"" : news.Title;
        }

        private string GetNumericAsDelta(int value)
        {
            return value > 0 ? "+" + value : value.ToString(CultureInfo.InvariantCulture);
        }
    }
}