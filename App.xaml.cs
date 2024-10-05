

using Microsoft.Extensions.Configuration;

namespace IndoorCO2App_Multiplatform
{
    public partial class App : Application
    {
        public App()
        {

            //currently it uses Syncfusion for the Rangeslider, which requires a license which is free as long as your revenue is below some millions
            //however including the serial key directly is not allowed so the file containing the key is not included. 
            //You need to either create your own account and your own key or replace the RangeSlider control.
            //It still works without license but shows an Info Popup at start of the App
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SerialKey.SyncFusionKey);
            InitializeComponent();

            MainPage = new AppShell();
            Shell.SetNavBarIsVisible(MainPage, false);
        }
    }
}
