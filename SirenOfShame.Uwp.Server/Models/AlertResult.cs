namespace SirenOfShame.Uwp.Server.Models
{
    internal sealed class AlertResult : SocketResult
    {
        public AlertResult(string message)
        {
            ResponseCode = 200;
            Type = "alertResult";
            Result = message;
        }
    }
}