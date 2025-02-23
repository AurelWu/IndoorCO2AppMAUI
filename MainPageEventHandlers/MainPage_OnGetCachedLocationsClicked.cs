

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnGetCachedLocationsClicked(object sender, EventArgs e)
        {
            if(SpatialManager.currentLocation!=null)
            {
                //Logger.circularBuffer.Add("Update Location started for coordinates: " + SpatialManager.currentLocation.Latitude + " | " + SpatialManager.currentLocation.Longitude);
            }
            if (currentMenuMode.HasFlag(MenuMode.Standard))
            {                
                OverpassModule.GetNearbyCachedBuildingLocations(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange);
                UpdateLocationPicker();
            }
            else if (currentMenuMode.HasFlag(MenuMode.TransportSelection))
            {
                OverpassModule.GetNearbyCachedTransitstopLocations(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange, true);
                UpdateTransitOriginPicker();
                OverpassModule.GetNearbyCachedTransitLineLocations(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange + 100);
                UpdateTransitLinesPicker();

            }
            else if (currentMenuMode.HasFlag(MenuMode.TransportRecording))
            {
                OverpassModule.GetNearbyCachedTransitstopLocations(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange, false);
                UpdateTransitDestinationPicker();
                OverpassModule.GetNearbyCachedTransitLineLocations(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange + 100);
                UpdateTransitLinesPicker();
            }
        }
    }

}
