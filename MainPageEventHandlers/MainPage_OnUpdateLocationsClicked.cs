

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnUpdateLocationsClicked(object sender, EventArgs e)
        {
            try
            {
                OverpassModule.lastFetchWasFromCachedData = false;
                if (SpatialManager.currentLocation != null)
                {
                    //Logger.circularBuffer.Add("Update Location started for coordinates: " + SpatialManager.currentLocation.Latitude + " | " + SpatialManager.currentLocation.Longitude);
                }
                if (currentMenuMode.HasFlag(MenuMode.Standard))
                {
                    OverpassModule.FetchNearbyBuildings(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange, this);
                }
                else if (currentMenuMode.HasFlag(MenuMode.TransportSelection))
                {
                    OverpassModule.FetchNearbyTransit(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange, this, true);
                }
                else if (currentMenuMode.HasFlag(MenuMode.TransportRecording))
                {
                    OverpassModule.FetchNearbyTransit(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, 250, this, false);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnUpdateLocationsClicked: {ex}", false);
            }
        }
    }

}
