

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
#if ANDROID
            MainPage = new AppShell();
            Shell.SetNavBarIsVisible(this, false);
#endif
//Shell.SetNavBarIsVisible(this, false);

#if IOS
            //Shell.SetNavBarIsVisible(this, false);
            MainPage = new MainPage();
#endif

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Logger.LogError("AppDomain Exception", e.ExceptionObject as Exception);
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                LogError("Unobserved Task Exception", e.Exception);
                e.SetObserved();
            };
        }




        protected override void OnSleep()
        {
            // Save state to Preferences
            Preferences.Set("OnSleepTriggeredTime", DateTime.Now.ToString());            
            Preferences.Set("TriedResumeAfterSleep", "false");
            SpatialManager.CancelGPSUpdateRequest(); //we
            BluetoothManager.CancelScanRequest();
        }

        protected override void OnResume()
        {
            Logger.WriteToLog("OnResume triggered" , false);
            Logger.WriteToLog("OnResume | before Resume Recording called",false);
            try
            {
                ResumeRecording();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Exception caused by ResumeRecording() " + ex.Source + " | " + ex.Message,false);
            }

            Logger.WriteToLog("OnResume | After Resume Recording called", false);
            Logger.WriteToLog("OnResume | Before GetCachedLocation called", false);
            try
            {
                SpatialManager.GetCachedLocation();
            }            
            catch (Exception ex)
            {
                Logger.WriteToLog("Exception caused by GetCachedLocation()",true);
            }
            Logger.WriteToLog("OnResume | After GetCachedLocation called",false);
        }

        public static void ResumeRecording()
        {
            string ost = Preferences.Get("OnSleepTriggeredTime", "never");
            Logger.WriteToLog("OnSleep triggered at " + ost,false);
            Logger.WriteToLog("OnResume triggered",false);
            string monitorType = Preferences.Get(RecoveryData.prefCO2MonitorType, "");
            string recordingMode = Preferences.Get(RecoveryData.prefRecoveryRecordingMode, "");
            Logger.WriteToLog("Stored MonitorType: " + monitorType,false);
            if (monitorType == CO2MonitorType.Aranet4.ToString() || monitorType == CO2MonitorType.Airvalent.ToString())
            {
                Logger.WriteToLog("checking if a resume attempt failed previously...",false);
                if (Preferences.Get("TriedResumeAfterSleep", "false") == "true") return;
                Logger.WriteToLog("resume attempt did not fail previously...",false);
                Logger.WriteToLog("checking if the app is already recording, meaning it didnt get reset...", false);
                if (BluetoothManager.isRecording) return; //means the app didnt get reset
                Logger.WriteToLog("app is not recording...",false);
                Logger.WriteToLog("checking if recording mode is either building or transit...", false);
                Logger.WriteToLog("recordingMode is: " + recordingMode, false);
                if (!(recordingMode == "Building" || recordingMode == "Transit")) return;
                Logger.WriteToLog("recording mode is either building or transit...",false);
                Logger.WriteToLog("recovering after app got stopped", false);

                //TODO => IF RECOVERY FAILED FOR WHATEVER REASON... dont try to recover again to prevent eternal problems
                Preferences.Set("TriedResumeAfterSleep", "true"); //if something goes wrong then next try it wont get called as long as there wasn't a OnSleep Before
                string mode = recordingMode;
                if (mode == "Building")
                {
                    Logger.WriteToLog("recovering Building Recording", false);
                    IndoorCO2App_Multiplatform.MainPage.MainPageSingleton.StartRecording(SubmissionMode.Building, true);
                }
                else if (mode == "Transit")
                {
                    Logger.WriteToLog("recovering Transit Recording",false);
                    IndoorCO2App_Multiplatform.MainPage.MainPageSingleton.StartTransportRecording(true);
                }
            }
        }
    }
}
