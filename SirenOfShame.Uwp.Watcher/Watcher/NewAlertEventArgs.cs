using System;
using System.Collections.Generic;
using System.Linq;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class NewAlertEventArgs
    {
        public string Message { get; set; }
        public string Url { get; set; }
        public int SoftwareInstanceId { get; set; }
        public DateTime AlertDate { get; set; }

        private static readonly ILog _log = MyLogManager.GetLog(typeof(RulesEngine));

        public bool Instantiate(string result)
        {
            List<string> results = result
                .Split('\n', '\r')
                .Where(i => !string.IsNullOrEmpty(i))
                .ToList();
            if (results.Count < 4)
            {
                _log.Error("Unable to parse alert response: " + result);
                return false;
            }

            try
            {
                SoftwareInstanceId = int.Parse(results.ElementAt(0));
                Url = results.ElementAtOrDefault(1);
                Message = results.ElementAtOrDefault(2);
                AlertDate = new DateTime(long.Parse(results.ElementAt(3)));

                return true;
            } 
            catch (Exception ex)
            {
                _log.Error("Error while parsing alert response: " + result, ex);
                return false;
            }
        }
    }
}