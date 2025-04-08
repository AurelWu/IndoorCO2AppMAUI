

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async Task OnUpdateLocationsClickedAsync(object sender, EventArgs e)
        {
            OverpassModule.lastFetchWasFromCachedData = false;
            if (SpatialManager.currentLocation!=null)
            {
                //Logger.circularBuffer.Add("Update Location started for coordinates: " + SpatialManager.currentLocation.Latitude + " | " + SpatialManager.currentLocation.Longitude);
            }
            if (currentMenuMode.HasFlag(MenuMode.Standard))
            {
                await OverpassModule.FetchNearbyBuildingsAsync(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange, this);
            }
            else if (currentMenuMode.HasFlag(MenuMode.TransportSelection))
            {
                await OverpassModule.FetchNearbyTransitAsync(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange, this, true);
            }
            else if (currentMenuMode.HasFlag(MenuMode.TransportRecording))
            {
                await OverpassModule.FetchNearbyTransitAsync(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, 250, this, false);
            }
        }
    }

}
