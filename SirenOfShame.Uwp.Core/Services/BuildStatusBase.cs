using System;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Core.Services
{
    public class BuildStatusBase
    {
        public DateTime? StartedTime { get; set; }
        public DateTime? FinishedTime { get; set; }

        public string RequestedBy { get; set; }
        public DateTime LocalStartTime { get; set; }
        public string BuildDefinitionId { get; set; }
        public string Name { get; set; }
        public string BuildId { get; set; }
        public string Url { get; set; }
        public BuildStatusEnum BuildStatusEnum { get; set; }
        public string BuildStatusMessage { get; set; }
        public string Comment { get; set; }
    }
}