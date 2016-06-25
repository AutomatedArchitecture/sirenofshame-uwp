namespace SirenOfShame.Uwp.Server.Models
{
    internal sealed class SirenInfoResult : SocketResult
    {
        public SirenInfoResult(SirenInfo sirenInfo)
        {
            ResponseCode = 200;
            Type = "getSirenInfoResult";
            Result = sirenInfo;
        }
    }
}