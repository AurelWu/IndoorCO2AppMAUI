﻿

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private void OnImprintClicked(object sender, EventArgs e)
        {
            var url = "https://www.indoorco2map.com/impressum.html";
            Launcher.OpenAsync(url);
        }

    }

}
