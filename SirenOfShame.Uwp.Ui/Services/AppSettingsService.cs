using Windows.Foundation.Collections;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class AppSettingsService
    {
        private const string ISVERYFIRSTLOAD = "IsVeryFirstLoad";

        public bool? IsVeryFirstLoad
        {
            get
            {
                return LocalSettingsValues[ISVERYFIRSTLOAD] as bool?;
            }
            set { LocalSettingsValues[ISVERYFIRSTLOAD] = value; }
        }

        private static IPropertySet LocalSettingsValues
        {
            get { return Windows.Storage.ApplicationData.Current.LocalSettings.Values; }
        }
    }
}
