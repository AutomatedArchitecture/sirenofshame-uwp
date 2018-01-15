using SirenOfShame.Uwp.Core.Models;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class SosOnlinePerson : PersonBase
    {
        public override string RawName { get; set; }
        public override string DisplayName { get; set; }
        public int AvatarId { get; set; }
        //public override int GetAvatarId(ImageList avatarImageList)
        //{
        //    return AvatarId;
        //}
    }
}