// In Platforms/Windows/LocationServiceWindows.cs


namespace IndoorCO2App
{
    internal class LocationServiceWindows : LocationService
    {
        internal override bool IsGpsEnabled()
        {
            return false;
            //var accessStatus = Geolocator.RequestAccessAsync().GetAwaiter().GetResult();
            //return accessStatus == GeolocationAccessStatus.Allowed;
        }
    }
}