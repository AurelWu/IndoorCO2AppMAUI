using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IndoorCO2App_Multiplatform
{
    internal static class SpatialManager
    {
        private static CancellationTokenSource gpsCancelTokenSource;
        private static bool isCheckingLocation;

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

        internal static async Task GetCachedLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    currentLocation = location;
                    //Logger.circularBuffer.Add("current Location set from cache: " + currentLocation.Latitude + "|" + currentLocation.Longitude); //TODO remove again
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Logger.circularBuffer.Add("getting cached GPS location not successful: " + fnsEx.Message);
            }
            catch (FeatureNotEnabledException fneEx)
            {

                Logger.circularBuffer.Add("getting cached GPS location not successful: " + fneEx.Message);
            }
            catch (PermissionException pEx)
            {
                Logger.circularBuffer.Add("getting cached GPS location not successful: " + pEx.Message);
            }
            catch (Exception ex)
            {
                Logger.circularBuffer.Add("getting cached GPS location not successful: " + ex.Message);
            }

        }

        internal static async void UpdateLocation()
        {

            await GetCachedLocation();
            

            try
            {
                isCheckingLocation = true;

                Logger.circularBuffer.Add("Requesting GPS Update | " + DateTime.Now);
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(15));
                gpsCancelTokenSource = new CancellationTokenSource();
                var result = await Geolocation.GetLocationAsync(request,gpsCancelTokenSource.Token);
                if (result != null)
                {
                    currentLocation = result;
                    //Logger.circularBuffer.Add("current Location set from UpdateLocation: " + currentLocation.Latitude + "|" + currentLocation.Longitude); //TODO remove again
                }
                else
                {
                    Logger.circularBuffer.Add("GPS update not successful:  null value after timeout: " + System.DateTime.Now);
                }
                locationUpdateSuccessful = true;
            }

            catch (FeatureNotSupportedException fnsEx)
            {
                Logger.circularBuffer.Add("GPS update not successful: " + fnsEx.Message);                
                locationUpdateSuccessful = false;
            }
            catch (FeatureNotEnabledException fneEx)
            {

                Logger.circularBuffer.Add("GPS update not successful: " + fneEx.Message);
                locationUpdateSuccessful = false;
            }
            catch (PermissionException pEx)
            {
                Logger.circularBuffer.Add("GPS update not successful: " + pEx.Message);
                locationUpdateSuccessful = false;
            }
            catch (Exception ex)
            {
                Logger.circularBuffer.Add("GPS update not successful: " + ex.Message);
                locationUpdateSuccessful = false;
            }
            finally
            {
                isCheckingLocation = false;
                try
                {
                    gpsCancelTokenSource?.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.circularBuffer.Add("gpsCancelTokenSource?.Dispose() not successful: " + ex.Message);
                }
                
            }

        }

        public static void CancelGPSUpdateRequest()
        {
            try
            {
                 if (isCheckingLocation && gpsCancelTokenSource != null && gpsCancelTokenSource.IsCancellationRequested == false)
                    gpsCancelTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Logger.circularBuffer.Add("CancelGPSUpdateRequest() not successful" + ex.Message);
            }
           
        }
    }
}

