namespace SirenOfShame.Uwp.Web.Controllers
{
    internal class SirenInfoController : ControllerBase
    {
        public override string CommandName => "getSirenInfo";

        public override SocketResult Invoke(string frame)
        {
            var sirenInfo = new SirenInfo
            {
                LedPatterns = SirenService.Instance.LedPatterns,
                AudioPatterns = SirenService.Instance.AudioPatterns
            };

            return new SirenInfoResult(sirenInfo);
        }
    }
}