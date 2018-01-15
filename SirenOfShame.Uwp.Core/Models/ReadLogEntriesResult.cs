using System.Collections.Generic;
using SirenOfShame.Uwp.Core.Interfaces;

namespace SirenOfShame.Uwp.Core.Models
{
    public class ReadLogEntriesResult
    {
        public List<ILogEntry> Events { get; set; }
    }
}