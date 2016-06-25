namespace SirenOfShame.Uwp.Background.Models
{
    internal abstract class SocketResult
    {
        public int ResponseCode { get; set; }
        public string Type { get; set; }
        public object Result { get; set; }
    }
}