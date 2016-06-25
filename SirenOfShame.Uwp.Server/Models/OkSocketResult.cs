namespace SirenOfShame.Uwp.Server.Models
{
    internal class DeviceConnectionChangedResult : SocketResult
    {
        public DeviceConnectionChangedResult(bool isConnected)
        {
            ResponseCode = 200;
            Type = "deviceConnectionChanged";
            Result = isConnected;
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