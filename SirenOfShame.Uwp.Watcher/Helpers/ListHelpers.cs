using System;
using System.Collections.Generic;

namespace SirenOfShame.Uwp.Watcher.Helpers
{
    public static class ListHelpers
    {
        public static void ForEach<T>(this List<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
    }
}
