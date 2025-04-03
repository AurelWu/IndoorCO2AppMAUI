

using CommunityToolkit.Maui.Views;
using IndoorCO2App_Android;
using System.Collections.Generic;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnShowQuickGuideClicked(object sender, EventArgs e)
        {
            try
            {
                var popup = new QuickGuidePopUp();
                this.ShowPopup(popup);
            }
            catch (Exception ex)
            {
                Logger.LogError("PopupRequest", ex);
            }
            
            //TODO: display info text with images
        }            
    }

}
