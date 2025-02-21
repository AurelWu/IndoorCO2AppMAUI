

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
                //TODO
            }
            else if (currentMenuMode.HasFlag(MenuMode.TransportRecording))
            {
                //TODO
            }
        }
    }

}
