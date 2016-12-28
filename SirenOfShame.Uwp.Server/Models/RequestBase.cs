namespace SirenOfShame.Uwp.Server.Models
{
    public class Request<T> : RequestBase
    {
        public T Message { get; set; }
    }

    public class RequestBase
    {
        public string Type { get; set; }
    }
}