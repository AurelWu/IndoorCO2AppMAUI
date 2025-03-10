﻿using System;
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
            OnStartListening();
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

        internal static async void ResetLocation()
        {
            await GetCachedLocation();
            if (OverpassModule.BuildingLocationData != null)
            {
                OverpassModule.BuildingLocationData.Clear();
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
                Logger.WriteToLog("getting cached GPS location not successful: " + fnsEx.Message, false);
            }
            catch (FeatureNotEnabledException fneEx)
            {

                Logger.WriteToLog("getting cached GPS location not successful: " + fneEx.Message, false);
            }
            catch (PermissionException pEx)
            {
                Logger.WriteToLog("getting cached GPS location not successful: " + pEx.Message, false);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("getting cached GPS location not successful: " + ex.Message, false);
            }

        }

        internal static async void UpdateLocation()
        {

            await GetCachedLocation();
            
            try
            {
                isCheckingLocation = true;

                Logger.WriteToLog("Requesting GPS Update |", false);
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(20));
                gpsCancelTokenSource = new CancellationTokenSource();
                var result = await Geolocation.GetLocationAsync(request,gpsCancelTokenSource.Token);
                if (result != null)
                {
                    currentLocation = result;
                    //Logger.circularBuffer.Add("current Location set from UpdateLocation: " + currentLocation.Latitude + "|" + currentLocation.Longitude); //TODO remove again
                }
                else
                {
                    Logger.WriteToLog("GPS update not successful:  null value after timeout: ",false);
                }
                locationUpdateSuccessful = true;
            }

            catch (FeatureNotSupportedException fnsEx)
            {
                Logger.WriteToLog("GPS update not successful: " + fnsEx.Message,false);                
                locationUpdateSuccessful = false;
            }
            catch (FeatureNotEnabledException fneEx)
            {

                Logger.WriteToLog("GPS update not successful: " + fneEx.Message, false);
                locationUpdateSuccessful = false;
            }
            catch (PermissionException pEx)
            {
                Logger.WriteToLog("GPS update not successful: " + pEx.Message, false);
                locationUpdateSuccessful = false;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("GPS update not successful: " + ex.Message, false);
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
                    Logger.WriteToLog("gpsCancelTokenSource?.Dispose() not successful: " + ex.Message, false);
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
                Logger.WriteToLog("CancelGPSUpdateRequest() not successful" + ex.Message, false);
            }
           
        }

        async static void OnStartListening()
        {
#if ANDROID
            try
            {
                Geolocation.LocationChanged -= Geolocation_LocationChanged;
                GeolocationAccuracy accuracy = GeolocationAccuracy.Best;
                Geolocation.LocationChanged += Geolocation_LocationChanged;
                var request = new GeolocationListeningRequest(accuracy);
                var success = await Geolocation.StartListeningForegroundAsync(request);

                string status = success
                    ? "Started listening for foreground location updates"
                    : "Couldn't start listening";
            }
            catch (Exception ex)
            {
                Logger.circularBuffer.Add("OnStartListening() not successful" + ex.Message);
            }
#endif
        }

        static void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            currentLocation = e.Location;
        }
    }
}

