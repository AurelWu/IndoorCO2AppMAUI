﻿#if ANDROID
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Provider;
using System.Threading.Tasks;

namespace IndoorCO2App_Android
{
    internal class LocationService
    {
        internal bool IsGpsEnabled()
        {
            LocationManager locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
        }

        internal async Task<bool> ShowEnableGpsDialogAsync()
        {
            bool result = await App.Current.MainPage.DisplayAlert(
                "Enable GPS",
                "GPS is currently disabled. Would you like to enable it?",
                "Yes",
                "No");

            if (result)
            {
                Intent intent = new Intent(Settings.ActionLocationSourceSettings);
                Platform.CurrentActivity.StartActivity(intent);
            }

            return result;
        }
    }
}
#endif