using System;
using SirenOfShame.Uwp.Core.Interfaces;

namespace SirenOfShame.Uwp.Core.Services
{
    public class MyLogManager
    {
        public static Func<Type, ILog> GetLog;
    }
}
