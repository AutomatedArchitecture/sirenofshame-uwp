namespace SirenOfShame.Uwp.Background.Models
{
    internal class OkSocketResult : SocketResult
    {
        public OkSocketResult()
        {
            ResponseCode = 200;
            Type = "OK";
            Result = "OK";
        }
    }
}