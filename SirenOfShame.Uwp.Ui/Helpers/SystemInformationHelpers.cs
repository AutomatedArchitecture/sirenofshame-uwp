namespace SirenOfShame.Uwp.Ui.Triggers
{
    public class SystemInformationHelpers
    {
        // Calculate this once and cache the result.
        private static bool _isXbox = (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox");

        // For now, the 10-foot experience is enabled only on Xbox.
        public static bool IsTenFootExperience => _isXbox;
    }
}
