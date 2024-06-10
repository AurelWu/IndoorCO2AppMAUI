using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IndoorCO2App
{
    internal static class SpatialManager
    {
        internal static Location currentLocation;
        internal static bool locationUpdateSuccessful;
        // Method to check if location permission is granted
        static LocationService ls;
        static SpatialManager()
        {
            #if ANDROID        
                ls = new LocationServiceAndroid();
            #elif IOS
                ls = new LocationServiceiOS();
            #elif WINDOWS
                ls = new LocationServiceWindows();
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

        internal static async void UpdateLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var result = await Geolocation.GetLocationAsync(request);
                if(result != null)
                {
                    currentLocation = result;
                }
                locationUpdateSuccessful = true;
            }

            catch (FeatureNotSupportedException fnsEx)
            {
                locationUpdateSuccessful = false; //maybe replace with enum?                
            }
            catch (FeatureNotEnabledException fneEx)
            {
                locationUpdateSuccessful = false; //maybe replace with enum?
            }
            catch (PermissionException pEx)
            {
                locationUpdateSuccessful = false; //maybe replace with enum?
            }
            catch (Exception ex)
            {
                locationUpdateSuccessful = false; //maybe replace with enum?
            }

        }
    }
}
