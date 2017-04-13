using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Util;

namespace SirenOfShame.Uwp.Ui.Models
{
    public class LeadersViewModel
    {
        public ObservableCollection<PersonDto> Leaders { get; set; }

        public void StatsChanged(IList<PersonSetting> changedPeople)
        {
            var leaderPairs = from oldLeader in Leaders
                join newLeader in changedPeople on oldLeader.RawName equals newLeader.RawName
                select new { oldLeader, newLeader };
            foreach (var leaderPair in leaderPairs)
            {
                leaderPair.oldLeader.Update(leaderPair.newLeader);
            }
            ResortLeaders();
        }

        private void ResortLeaders()
        {
            Leaders.SortDescending(i => i.Reputation);
        }

        public void AddPerson(List<PersonSetting> newPeople)
        {
            foreach (var personSetting in newPeople)
            {
                var personDto = new PersonDto(personSetting);
                Leaders.Add(personDto);
            }
            ResortLeaders();
        }
    }
}