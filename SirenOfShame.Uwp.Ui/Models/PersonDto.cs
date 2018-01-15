using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Ui.Annotations;

namespace SirenOfShame.Uwp.Ui.Models
{
    public class PersonDto : INotifyPropertyChanged
    {
        private string _totalBuilds;
        private string _fixedSomeoneElsesBuild;
        private string _successfulBuildsInARow;
        private string _failPercent;
        private int _achievements;
        private int _reputation;
        private string _displayName;
        private string _rawName;
        private int? _avatarId;

        public PersonDto(PersonSettingBase person)
        {
            Update(person);
        }

        public void Update(PersonSettingBase person)
        {
            RawName = person.RawName;
            DisplayName = person.DisplayName;
            AvatarId = person.AvatarId;
            Reputation = person.GetReputation();
            Achievements = person.Achievements.Count;
            AchievementIdsList = person.Achievements.Select(i => i.AchievementId).ToList();
            FailPercent = string.Format("{0:p1}", person.CurrentBuildRatio).Replace(" ", "");
            SuccessfulBuildsInARow = string.Format("{0}", person.CurrentSuccessInARow);
            FixedSomeoneElsesBuild = string.Format("{0}", person.NumberOfTimesFixedSomeoneElsesBuild);
            TotalBuilds = string.Format("{0}", person.TotalBuilds);
        }

        public List<int> AchievementIdsList { get; set; }

        public int? AvatarId
        {
            get { return _avatarId; }
            set
            {
                if (value == _avatarId) return;
                _avatarId = value;
                OnPropertyChanged();
            }
        }

        public string TotalBuilds
        {
            get { return _totalBuilds; }
            set
            {
                if (value == _totalBuilds) return;
                _totalBuilds = value;
                OnPropertyChanged();
            }
        }

        public string FixedSomeoneElsesBuild
        {
            get { return _fixedSomeoneElsesBuild; }
            set
            {
                if (value == _fixedSomeoneElsesBuild) return;
                _fixedSomeoneElsesBuild = value;
                OnPropertyChanged();
            }
        }

        public string SuccessfulBuildsInARow
        {
            get { return _successfulBuildsInARow; }
            set
            {
                if (value == _successfulBuildsInARow) return;
                _successfulBuildsInARow = value;
                OnPropertyChanged();
            }
        }

        public string FailPercent
        {
            get { return _failPercent; }
            set
            {
                if (value == _failPercent) return;
                _failPercent = value;
                OnPropertyChanged();
            }
        }

        public int Achievements
        {
            get { return _achievements; }
            set
            {
                if (value == _achievements) return;
                _achievements = value;
                OnPropertyChanged();
            }
        }

        public int Reputation
        {
            get { return _reputation; }
            set
            {
                if (value == _reputation) return;
                _reputation = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public string RawName
        {
            get { return _rawName; }
            set
            {
                if (value == _rawName) return;
                _rawName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
}
}
