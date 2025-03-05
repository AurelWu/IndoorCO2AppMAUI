

using CommunityToolkit.Maui.Views;
using IndoorCO2App_Android;
using Microsoft.VisualStudio.Utilities;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnCrashLogButtonClicked(object sender, EventArgs e)
        {
            try
            {
                PersistentLogHelper.CopyCrashLogToClipboardAsync();
            }
            catch
            {
                Logger.WriteToLog("Copying Crashlog to Clipboard failed",false);
            }
            
        }


    }

}
