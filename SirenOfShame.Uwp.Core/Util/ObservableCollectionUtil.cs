using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SirenOfShame.Uwp.Core.Models;

namespace SirenOfShame.Uwp.Core.Util
{
    public static class ObservableCollectionUtil
    {
        public static void SortDescending<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> predicate)
        {
            var sorted = collection.OrderByDescending(predicate).ToList();
            SortTo(collection, sorted);
        }

        public static void Sort<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> predicate)
        {
            var sorted = collection.OrderBy(predicate).ToList();
            SortTo(collection, sorted);
        }

        private static void SortTo<T>(this ObservableCollection<T> collection, List<T> sorted)
        {
            for (int i = 0; i < sorted.Count; i++)
            {
                var oldIndex = collection.IndexOf(sorted[i]);
                if (oldIndex != i)
                {
                    collection.Move(oldIndex, i);
                }
            }
        }

        // todo: make this generic
        public static void Sort(this ObservableCollection<BuildStatusDto> list)
        {
            list.SortDescending(i => i.LocalStartTime);
        }
    }
}
