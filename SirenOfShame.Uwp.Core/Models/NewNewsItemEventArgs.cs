using System.Collections.Generic;

namespace SirenOfShame.Uwp.Core.Models
{
    public class NewNewsItemEventArgs
    {
        public const string COMMAND_NAME = "NewNewsItem";
        public List<NewsItemEvent> NewsItemEvents { get; set; }
    }
}