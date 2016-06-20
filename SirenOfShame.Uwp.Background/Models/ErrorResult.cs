namespace SirenOfShame.Uwp.Background.Models
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