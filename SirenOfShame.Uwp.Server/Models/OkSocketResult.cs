namespace SirenOfShame.Uwp.Server.Models
{
    internal class OkSocketResult<T> : SocketResult
    {
        public OkSocketResult(T result)
        {
            ResponseCode = 200;
            Type = "OK";
            Result = result;
        }
    }

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