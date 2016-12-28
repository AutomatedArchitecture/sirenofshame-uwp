using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SirenOfShame.Uwp.Watcher.Annotations;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class BuildStatusDto : INotifyPropertyChanged
    {
        private string _buildDefinitionDisplayName;
        private BuildStatusEnum _buildStatusEnum;
        private string _buildStatusMessage;
        private string _comment;
        private string _duration;
        private int _imageIndex;
        private DateTime _localStartTime;
        private string _requestedByDisplayName;
        private string _url;

        public string BuildDefinitionDisplayName
        {
            get { return _buildDefinitionDisplayName; }
            set
            {
                if (value == _buildDefinitionDisplayName) return;
                _buildDefinitionDisplayName = value;
                OnPropertyChanged();
            }
        }

        public BuildStatusEnum BuildStatusEnum
        {
            get { return _buildStatusEnum; }
            set
            {
                if (value == _buildStatusEnum) return;
                _buildStatusEnum = value;
                OnPropertyChanged();
            }
        }

        public string BuildStatusMessage
        {
            get { return _buildStatusMessage; }
            set
            {
                if (value == _buildStatusMessage) return;
                _buildStatusMessage = value;
                OnPropertyChanged();
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                if (value == _comment) return;
                _comment = value;
                OnPropertyChanged();
            }
        }

        public string Duration
        {
            get { return _duration; }
            set
            {
                if (value == _duration) return;
                _duration = value;
                OnPropertyChanged();
            }
        }

        public int ImageIndex
        {
            get { return _imageIndex; }
            set
            {
                if (value == _imageIndex) return;
                _imageIndex = value;
                OnPropertyChanged();
            }
        }

        public DateTime LocalStartTime
        {
            get { return _localStartTime; }
            set
            {
                if (value.Equals(_localStartTime)) return;
                _localStartTime = value;
                OnPropertyChanged();
            }
        }

        public string RequestedByDisplayName
        {
            get { return _requestedByDisplayName; }
            set
            {
                if (value == _requestedByDisplayName) return;
                _requestedByDisplayName = value;
                OnPropertyChanged();
            }
        }

        public string Url
        {
            get { return _url; }
            set
            {
                if (value == _url) return;
                _url = value;
                OnPropertyChanged();
            }
        }

        public string RequestedByRawName { get; set; }
        public string StartTimeShort { get; set; }
        public string BuildDefinitionId { get; set; }
        public string BuildId { get; set; }

        public void SetDisplayName(SirenOfShameSettings settings)
        {
            var person = settings.FindAddPerson(RequestedByRawName);
            RequestedByDisplayName = person == null ? RequestedByRawName : person.DisplayName;
        }

        public void Update(BuildStatusDto newBd)
        {
            BuildStatusEnum = newBd.BuildStatusEnum;
            BuildDefinitionDisplayName = newBd.BuildDefinitionDisplayName;
            LocalStartTime = newBd.LocalStartTime;
            ImageIndex = newBd.ImageIndex;
            BuildStatusMessage = newBd.BuildStatusMessage;
            Comment = newBd.Comment;
            Duration = newBd.Duration;
            RequestedByDisplayName = newBd.RequestedByDisplayName;
            Url = newBd.Url;
    }

    public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
