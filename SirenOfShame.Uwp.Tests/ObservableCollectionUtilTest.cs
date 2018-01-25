using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Core.Util;
using SirenOfShame.Uwp.Watcher.Watcher;
using SirenOfShame.Uwp.Watcher.Util;

namespace SirenOfShame.Uwp.Tests
{
    [TestFixture]
    public class ObservableCollectionUtilTest
    {
        [Test]
        public void GivenThreeElementsMostRecentOnBottom_WhenSort_ThenMostRecentOnTop()
        {
            ObservableCollection<BuildStatusDto> list = new ObservableCollection<BuildStatusDto>
            {
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 1) },
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 2) },
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 3) }
            };
            list.Sort();
            Assert.AreEqual(new DateTime(2015, 1, 3), list[0].LocalStartTime);
            Assert.AreEqual(new DateTime(2015, 1, 2), list[1].LocalStartTime);
        }

        [Test]
        public void GivenThreeElementsWithMostRecentOnTop_WhenSort_ThenMostRecentStillOnTop()
        {
            ObservableCollection<BuildStatusDto> list = new ObservableCollection<BuildStatusDto>
            {
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 3) },
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 2) },
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 1) }
            };
            list.Sort();
            Assert.AreEqual(new DateTime(2015, 1, 3), list[0].LocalStartTime);
            Assert.AreEqual(new DateTime(2015, 1, 2), list[1].LocalStartTime);
        }
    }
}
