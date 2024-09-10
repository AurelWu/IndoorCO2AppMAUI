

using Microsoft.Extensions.Configuration;

namespace IndoorCO2App_Android
{
    public partial class App : Application
    {
        public App()
        {


            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SerialKey.SyncFusionKey);
            InitializeComponent();

            MainPage = new AppShell();
            Shell.SetNavBarIsVisible(MainPage, false);
        }
    }
}
