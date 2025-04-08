

using CommunityToolkit.Maui.Views;
using IndoorCO2App_Android;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async Task OnImprintClickedAsync(object sender, EventArgs e)
        {


            var url = "https://www.indoorco2map.com/impressum.html";
            await Launcher.OpenAsync(url);
        }

    }

}
