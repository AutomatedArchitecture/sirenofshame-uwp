namespace SirenOfShame.Uwp.Server.Models
{
    internal sealed class ErrorResult : SocketResult
    {
        public ErrorResult(int responseCode, string message)
        {
            ResponseCode = responseCode;
            Result = message;
        }
    }
}