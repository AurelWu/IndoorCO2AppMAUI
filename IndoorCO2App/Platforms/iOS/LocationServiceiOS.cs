// In Platforms/iOS/LocationServiceiOS.cs
//using CoreLocation;
//using Foundation;
//using UIKit;
//
//
//namespace IndoorCO2App
//{
//    internal class LocationServiceiOS : LocationService
//    {
//        internal override bool IsGpsEnabled()
//        {
//            return CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse ||
//                   CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways;
//        }
//
//        internal override async Task<bool> ShowEnableGpsDialogAsync()
//        {
//            bool result = await App.Current.MainPage.DisplayAlert(
//                "Enable GPS",
//                "GPS is currently disabled. Would you like to enable it?",
//                "Yes",
//                "No");
//
//            if (result)
//            {
//                var url = new NSUrl("App-Prefs:root=LOCATION_SERVICES");
//                if (UIApplication.SharedApplication.CanOpenUrl(url))
//                {
//                    UIApplication.SharedApplication.OpenUrl(url);
//                }
//            }
//
//            return result;
//        }
//    }
//}