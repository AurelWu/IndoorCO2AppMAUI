#if IOS
using CoreLocation;
using Foundation;
using System.Threading.Tasks;
using UIKit;

namespace IndoorCO2App_Multiplatform
{
    internal class LocationServiceApple : ILocationService
    {
        private CLLocationManager locationManager;

        public LocationServiceApple()
        {
            locationManager = new CLLocationManager();
            //locationManager.AllowsBackgroundLocationUpdates = true;
        }

        public bool IsGpsEnabled()
        {
            return CLLocationManager.LocationServicesEnabled;
        }

        public async Task<bool> ShowEnableGpsDialogAsync()
        {
            if (!IsGpsEnabled())
            {
                var result = await App.Current.MainPage.DisplayAlert(
                    "Enable GPS",
                    "GPS is currently disabled. Would you like to enable it?",
                    "Yes",
                    "No");

                if (result)
                {
                    // Open the iOS Settings app to the location services section
                    var settingsUrl = new NSUrl("App-Prefs:root=Privacy&path=LOCATION");
                    if (UIApplication.SharedApplication.CanOpenUrl(settingsUrl))
                    {
                        UIApplication.SharedApplication.OpenUrl(settingsUrl);
                    }
                }

                return result;
            }

            return true; // GPS is already enabled
        }
    }
}
#endif