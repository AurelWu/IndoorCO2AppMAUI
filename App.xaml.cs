

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
            Logger.circularBuffer.Add("OnResume triggered at: " + DateTime.Now);
            Logger.circularBuffer.Add("OnResume | before Resume Recording called: " + DateTime.Now);
            try
            {
                ResumeRecording();
            }
            catch (Exception ex)
            {
                Logger.circularBuffer.Add("Exception caused by ResumeRecording()");
            }
            
            Logger.circularBuffer.Add("OnResume | After Resume Recording called: " + DateTime.Now);
            Logger.circularBuffer.Add("OnResume | Before GetCachedLocation called: " + DateTime.Now);
            try
            {
                SpatialManager.GetCachedLocation();
            }            
            catch (Exception ex)
            {
                Logger.circularBuffer.Add("Exception caused by GetCachedLocation()");
            }
            Logger.circularBuffer.Add("OnResume | After GetCachedLocation called: " + DateTime.Now);
        }

        public static void ResumeRecording()
        {
            string ost = Preferences.Get("OnSleepTriggeredTime", "never");
            Logger.circularBuffer.Add("OnSleep triggered at: " + ost);
            Logger.circularBuffer.Add("OnResume triggered at: " + DateTime.Now);
            string monitorType = Preferences.Get(RecoveryData.prefCO2MonitorType, "");
            string recordingMode = Preferences.Get(RecoveryData.prefRecoveryRecordingMode, "");
            Logger.circularBuffer.Add("Stored MonitorType: " + monitorType);
            if (monitorType == CO2MonitorType.Aranet4.ToString() || monitorType == CO2MonitorType.Airvalent.ToString())
            {
                Logger.circularBuffer.Add("checking if a resume attempt failed previously...");
                if (Preferences.Get("TriedResumeAfterSleep", "false") == "true") return;
                Logger.circularBuffer.Add("resume attempt did not fail previously...");
                Logger.circularBuffer.Add("checking if the app is already recording, meaning it didnt get reset...");
                if (BluetoothManager.isRecording) return; //means the app didnt get reset
                Logger.circularBuffer.Add("app is not recording...");
                Logger.circularBuffer.Add("checking if recording mode is either building or transit...");
                Logger.circularBuffer.Add("recordingMode is: " + recordingMode);
                if (!(recordingMode == "Building" || recordingMode == "Transit")) return;
                Logger.circularBuffer.Add("recording mode is either building or transit...");
                Logger.circularBuffer.Add("recovering after app got stopped");

                //TODO => IF RECOVERY FAILED FOR WHATEVER REASON... dont try to recover again to prevent eternal problems
                Preferences.Set("TriedResumeAfterSleep", "true"); //if something goes wrong then next try it wont get called as long as there wasn't a OnSleep Before
                string mode = recordingMode;
                if (mode == "Building")
                {
                    Logger.circularBuffer.Add("recovering Building Recording");
                    IndoorCO2App_Multiplatform.MainPage.MainPageSingleton.StartRecording(SubmissionMode.Building, true);
                }
                else if (mode == "Transit")
                {
                    Logger.circularBuffer.Add("recovering Transit Recording");
                    IndoorCO2App_Multiplatform.MainPage.MainPageSingleton.StartTransportRecording(true);
                }
            }
        }
    }
}
