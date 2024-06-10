// In Platforms/Android/LocationServiceAndroid.cs
using Android.Content;
using Android.Locations;
using AndroidApp = Android.App;


namespace IndoorCO2App
{
    internal class LocationServiceAndroid : LocationService
    {
        internal override bool IsGpsEnabled()
        {
            var locationManager = (LocationManager)AndroidApp.Application.Context.GetSystemService(Context.LocationService);
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
        }

        internal override async Task<bool> ShowEnableGpsDialogAsync()
        {
            bool result = await App.Current.MainPage.DisplayAlert(
                "Enable GPS",
                "GPS is currently disabled. Would you like to enable it?",
                "Yes",
                "No");

            if (result)
            {
                var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                Platform.CurrentActivity.StartActivity(intent);
            }

            return result;
        }
    }
}