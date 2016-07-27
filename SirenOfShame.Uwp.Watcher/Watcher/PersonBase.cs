namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public abstract class PersonBase
    {
        public abstract string RawName { get; set; }
        public abstract string DisplayName { get; set; }

        public bool Clickable
        {
            get { return RawName != null; }
        }

        //public abstract int GetAvatarId(ImageList avatarImageList);
    }
}