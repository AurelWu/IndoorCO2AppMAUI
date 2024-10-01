using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IndoorCO2App_Android
{
    internal static class SpatialManager
    {
        internal static Location currentLocation;
        internal static bool locationUpdateSuccessful;
        // Method to check if location permission is granted
        static ILocationService ls;
        static SpatialManager()
        {
#if ANDROID
            ls = new LocationService();
#endif
#if IOS
            ls = new LocationServiceApple();
#endif
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

        internal static void ResetLocation()
        {
            currentLocation = new Location();
            if (OverpassModule.LocationData != null)
            {
                OverpassModule.LocationData.Clear();
            }
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
                Debug.Print(fnsEx.Message);
                locationUpdateSuccessful = false;
            }
            catch (FeatureNotEnabledException fneEx)
            {

                Debug.Print(fneEx.Message);
                locationUpdateSuccessful = false;
            }
            catch (PermissionException pEx)
            {
                Debug.Print(pEx.Message);
                locationUpdateSuccessful = false;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                locationUpdateSuccessful = false;
            }

        }
    }
}

