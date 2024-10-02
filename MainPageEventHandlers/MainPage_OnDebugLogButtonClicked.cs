

using Microsoft.VisualStudio.Utilities;

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private void OnDebugLogButtonClicked(object sender, EventArgs e)
        {
            Logger.CopyLogToClipboard();   
        }


    }

}
