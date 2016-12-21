using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using SirenOfShame.Uwp.Watcher.Watcher;
using SirenOfShame.Uwp.Watcher.Util;

namespace SirenOfShame.Uwp.Tests
{
    [TestFixture]
    public class ObservableCollectionUtilTest
    {
        [Test]
        public void GivenEmptyList_WhenSortOnce_ThenFalse()
        {
            ObservableCollection<BuildStatusDto> list = new ObservableCollection<BuildStatusDto>();
            Assert.AreEqual(false, ObservableCollectionUtil.SortOnce(list));
        }

        [Test]
        public void GivenTwoElementsInOrder_WhenSortOnce_ThenFalse()
        {
            ObservableCollection<BuildStatusDto> list = new ObservableCollection<BuildStatusDto>
            {
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 2) },
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 1) }
            };
            Assert.AreEqual(false, ObservableCollectionUtil.SortOnce(list));
            Assert.AreEqual(new DateTime(2015, 1, 2), list[0].LocalStartTime);
        }

        [Test]
        public void GivenTwoElementsOutOfOrder_WhenSortOnce_ThenReSortedCorrectly()
        {
            ObservableCollection<BuildStatusDto> list = new ObservableCollection<BuildStatusDto>
            {
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 1) },
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 2) }
            };
            Assert.AreEqual(true, ObservableCollectionUtil.SortOnce(list));
        }

        [Test]
        public void GivenThreeElementsInReverseOrder_WhenSortOnce_ThenOneSortPerformed()
        {
            ObservableCollection<BuildStatusDto> list = new ObservableCollection<BuildStatusDto>
            {
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 1) },
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 2) },
                new BuildStatusDto { LocalStartTime = new DateTime(2015, 1, 3) }
            };
            var sortOnce = ObservableCollectionUtil.SortOnce(list);
            Assert.AreEqual(true, sortOnce);
            Assert.AreEqual(new DateTime(2015, 1, 2), list[0].LocalStartTime);
        }

        [Test]
        public void GivenThreeElementsInReverseOrder_WhenSort_ThenItemsInCorrectOrder()
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
    }
}
