

using CommunityToolkit.Maui.Views;
using IndoorCO2App_Android;
using Microsoft.VisualStudio.Utilities;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnDebugLogButtonClicked(object sender, EventArgs e)
        {
            Logger.CopyLogToClipboardAsync();   
        }


    }

}
