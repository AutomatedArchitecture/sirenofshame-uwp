using System.Collections.ObjectModel;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Util
{
    public static class ObservableCollectionUtil
    {
        // todo: make this generic
        public static void Sort(this ObservableCollection<BuildStatusDto> list)
        {
            bool sortPerformed;
            do
            {
                sortPerformed = SortOnce(list);
            } while (sortPerformed);
        }

        public static bool SortOnce(ObservableCollection<BuildStatusDto> list)
        {
            BuildStatusDto lastItem = null;
            for (int i = 0; i < list.Count; i++)
            {
                var currentItem = list[i];
                if (lastItem != null)
                {
                    var currentTime = currentItem.LocalStartTime;
                    var lastTime = lastItem.LocalStartTime;
                    var isInTheWrongOrder = currentTime > lastTime;
                    if (isInTheWrongOrder)
                    {
                        list.Remove(currentItem);
                        for (int j = 0; j < i; j++)
                        {
                            if (list[j].LocalStartTime < currentTime)
                            {
                                list.Insert(j, currentItem);
                                return true;
                            }
                        }
                    }
                }
                lastItem = currentItem;
            }
            return false;
        }

    }
}
