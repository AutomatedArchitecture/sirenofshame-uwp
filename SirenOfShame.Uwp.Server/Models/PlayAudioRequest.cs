namespace SirenOfShame.Uwp.Server.Models
{
    internal class PlayAudioRequest : RequestBase
    {
        public int? Id { get; set; }
        public int? Duration { get; set; }
    }
}