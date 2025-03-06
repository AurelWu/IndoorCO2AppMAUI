

using CommunityToolkit.Maui.Views;
using IndoorCO2App_Android;
using Microsoft.VisualStudio.Utilities;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnCrashLogButtonClicked(object sender, EventArgs e)
        {
            try
            {
                await PersistentLogHelper.CopyCrashLogToClipboardAsync();
            }
            catch
            {
                Logger.WriteToLog("Copying Crashlog to Clipboard failed",false);
            }
            
        }


    }

}
