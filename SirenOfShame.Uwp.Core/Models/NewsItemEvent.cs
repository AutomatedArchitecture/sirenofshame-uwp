using System;

namespace SirenOfShame.Uwp.Core.Models
{
    public class NewsItemEvent
    {
        public string BuildDefinitionId { get; set; }
        public DateTime EventDate { get; set; }
        public PersonSettingBase Person { get; set; }
        public string Title { get; set; }
        //public ImageList AvatarImageList { get; set; }
        public NewsItemTypeEnum NewsItemType { get; set; }
        public int? ReputationChange { get; set; }
        public string BuildId { get; set; }

        public bool IsSosOnlineEvent
        {
            get { return NewsItemType < NewsItemTypeEnum.BuildStarted; }
        }

        public bool ShouldUpdateOldInProgressNewsItem
        {
            get
            {
                return !string.IsNullOrEmpty(BuildId) && (
                           NewsItemType == NewsItemTypeEnum.BuildFailed ||
                           NewsItemType == NewsItemTypeEnum.BuildSuccess ||
                           NewsItemType == NewsItemTypeEnum.BuildUnknown
                       );
            }
        }
    }
}
