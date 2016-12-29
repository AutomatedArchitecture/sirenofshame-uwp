using System;
using SirenOfShame.Test.Unit.Services;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Test.Unit.Watcher
{
    class FakeRulesEngine : RulesEngine
    {
        public SosOnlineService MockSosOnlineService = new SosOnlineServiceDummy();

        DateTime _now = new DateTime(2010, 1, 2);

        public void SetNow(DateTime now)
        {
            _now = now;
        }
        
        protected override DateTime Now
        {
            get { return _now; }
        }

        protected override SosOnlineService SosOnlineService
        {
            get { return MockSosOnlineService; }
        }
    }
}
