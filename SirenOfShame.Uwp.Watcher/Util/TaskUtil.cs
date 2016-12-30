using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SirenOfShame.Uwp.Watcher.Util
{
    public static class TaskUtil
    {
        /// <summary>
        /// Disables CS4014. Adapted from http://stackoverflow.com/questions/22629951/suppressing-warning-cs4014-because-this-call-is-not-awaited-execution-of-the
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FireAndForget(this Task task)
        {
        }
    }
}
