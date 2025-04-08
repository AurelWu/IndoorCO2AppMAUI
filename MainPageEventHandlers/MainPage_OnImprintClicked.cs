

using CommunityToolkit.Maui.Views;
using IndoorCO2App_Android;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnImprintClicked(object sender, EventArgs e)
        {
            try
            {
                var url = "https://www.indoorco2map.com/impressum.html";
                await Launcher.OpenAsync(url);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnImprintClicked: {ex}", false);
            }

            
        }

    }

}
