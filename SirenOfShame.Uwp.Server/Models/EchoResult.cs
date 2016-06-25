namespace SirenOfShame.Uwp.Background.Models
{
    internal sealed class EchoResult : SocketResult
    {
        public EchoResult(string message)
        {
            ResponseCode = 200;
            Type = "echoResult";
            Result = message;
        }
    }
}