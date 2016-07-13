namespace SirenOfShame.Uwp.Server.Models
{
    internal class PlayLedRequest : RequestBase
    {
        public int? Id { get; set; }
        public int? Duration { get; set; }
    }
}