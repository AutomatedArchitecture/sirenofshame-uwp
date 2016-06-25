namespace SirenOfShame.Uwp.Server.Models
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