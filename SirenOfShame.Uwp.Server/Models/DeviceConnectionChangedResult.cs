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
}