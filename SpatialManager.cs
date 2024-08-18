using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IndoorCO2App_Android
{
    internal static class SpatialManager
    {
#if ANDROID
        internal static Location currentLocation;
        internal static bool locationUpdateSuccessful;
        // Method to check if location permission is granted
        static LocationService ls;
        static SpatialManager()
        {
            ls = new LocationService();
            currentLocation = new Location();
        }

        internal static async Task<bool> IsLocationPermissionGrantedAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }

        // Method to request location permission
        internal static async Task<bool> RequestLocationPermissionAsync()
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return status == PermissionStatus.Granted;
        }

        internal static bool CheckIfGpsIsEnabled()
        {
            return ls.IsGpsEnabled();
        }

        internal static async Task<bool> ShowEnableGPSDialogAsync()
        {
            return await ls.ShowEnableGpsDialogAsync();
        }

        internal static async void UpdateLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var result = await Geolocation.GetLocationAsync(request);
                if (result != null)
                {
                    currentLocation = result;
                }
                locationUpdateSuccessful = true;
            }

            catch (FeatureNotSupportedException fnsEx)
            {
                locationUpdateSuccessful = false;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                locationUpdateSuccessful = false;
            }
            catch (PermissionException pEx)
            {
                locationUpdateSuccessful = false;
            }
            catch (Exception ex)
            {
                locationUpdateSuccessful = false;
            }

        }
#endif
    }
}

