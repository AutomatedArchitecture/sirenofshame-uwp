using System.Collections.Generic;
using SirenOfShame.Device;

namespace SirenOfShame.Uwp.Background.Models
{
    internal class SirenInfo
    {
        public List<LedPattern> LedPatterns { get; set; }
        public List<AudioPattern> AudioPatterns { get; set; }
    }
}