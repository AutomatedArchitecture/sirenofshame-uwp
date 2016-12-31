using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using SirenOfShame.Uwp.Ui.Annotations;
using SirenOfShame.Uwp.Watcher.Helpers;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Models
{
    public class NewsItemDto : INotifyPropertyChanged
    {
        private string _title;
        private string _displayName;
        private string _project;
        private string _reputationChange;
        private string _when;
        private NewsItemTypeEnum _newsItemTypeEnum;

        public string BuildId { get; }
        private DateTime EventDate { get; set; }

        public string Title
        {
            get { return _title; }
            private set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            private set
            {
                if (value == _displayName) return;
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public string Project
        {
            get { return _project; }
            private set
            {
                if (value == _project) return;
                _project = value;
                OnPropertyChanged();
            }
        }

        public string ReputationChange
        {
            get { return _reputationChange; }
            private set
            {
                if (value == _reputationChange) return;
                _reputationChange = value;
                OnPropertyChanged();
            }
        }

        public string When
        {
            get { return _when; }
            private set
            {
                if (value == _when) return;
                _when = value;
                OnPropertyChanged();
            }
        }

        public NewsItemTypeEnum NewsItemTypeEnum
        {
            get { return _newsItemTypeEnum; }
            private set
            {
                if (value == _newsItemTypeEnum) return;
                _newsItemTypeEnum = value;
                OnPropertyChanged();
            }
        }

        public NewsItemDto(NewsItemEvent news)
        {
            BuildId = news.BuildId;
            UpdateState(news);
        }

        public void UpdateState(NewsItemEvent news)
        {
            ReputationChange = news.ReputationChange.HasValue ? GetNumericAsDelta(news.ReputationChange.Value) : null;
            Title = GetTitle(news);
            DisplayName = news.Person.DisplayName;
            Project = news.BuildDefinitionId?.ToUpperInvariant();
            EventDate = news.EventDate;
            NewsItemTypeEnum = news.NewsItemType;
            CalculatePrettyDate();
        }

        public void CalculatePrettyDate()
        {
            When = EventDate.PrettyDate();
        }

        private static string GetTitle(NewsItemEvent news)
        {
            return news.NewsItemType == NewsItemTypeEnum.SosOnlineComment ? "\"" + news.Title + "\"" : news.Title;
        }

        private string GetNumericAsDelta(int value)
        {
            return value > 0 ? "+" + value : value.ToString(CultureInfo.InvariantCulture);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}