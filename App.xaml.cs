
using IndoorCO2App_Android;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace IndoorCO2App_Multiplatform
{
    public partial class App : Application
    {
        public static string langString = "en";
        public static string userManualStringWithoutExtension = "https://indoorco2map.com/Manual";
        public static void SetCulture(string cultureString)
        {
            try
            {
                langString = cultureString;
                var culture = new CultureInfo(cultureString);
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                QuickGuidePopUp.userManualURL = userManualStringWithoutExtension + "_" + cultureString + ".pdf";
                if (langString == "en")
                {
                    QuickGuidePopUp.userManualURL = userManualStringWithoutExtension +".pdf";
                }

                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                LocalisationResourceManager.Instance.SetCulture(culture);
            }
            catch (CultureNotFoundException)
            {
                langString = "en";
                QuickGuidePopUp.userManualURL = userManualStringWithoutExtension + ".pdf";
                // Fallback to neutral (invariant) culture
                var fallback = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentUICulture = fallback;
                CultureInfo.DefaultThreadCurrentCulture = fallback;
                LocalisationResourceManager.Instance.SetCulture(fallback);
            }
        }

        public App()
        {
          
            //currently it uses Syncfusion for the Rangeslider, which requires a license which is free as long as your revenue is below some millions
            //however including the serial key directly is not allowed so the file containing the key is not included. 
            //You need to either create your own account and your own key or replace the RangeSlider control.
            //It still works without license but shows an Info Popup at start of the App
            try
            {
                string key = Environment.GetEnvironmentVariable("SYNC_FUSION_KEY");                
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(key);
            }
            catch
            {
                Logger.WriteToLog("error getting/setting syncfusion license key",true);
            }
            

            InitializeComponent();

            string applang = Preferences.Get("AppLanguage","en");


            var loc = LocalisationResourceManager.Instance;
            loc.Init(AppStrings.ResourceManager);
            // ✅ Im globalen ResourceDictionary registrieren
            Current.Resources["Loc"] = loc;
            App.SetCulture(applang);
            Initialize();
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
                Logger.LogError("Unobserved Task Exception", e.Exception);
                e.SetObserved();
            };

            // Catch UI Thread Exceptions
            Application.Current.Dispatcher.Dispatch(async () =>
            {
                try
                {
                    await Task.Delay(500); // Allow startup
                }
                catch (Exception ex)
                {
                    Logger.LogError("UI Thread Exception", ex);
                }
            });
        }

        private async void Initialize()
        {
            OverpassModule.Initialize();
        }


        protected override void OnSleep()
        {
            // Save state to Preferences
            Preferences.Set("OnSleepTriggeredTime", DateTime.Now.ToString());            
            Preferences.Set("TriedResumeAfterSleep", "false");
            SpatialManager.CancelGPSUpdateRequest(); //we
            BluetoothManager.CancelScanRequest();
        }

        protected override async void OnResume()
        {
            base.OnResume();

            // Offload the async work to a separate task
            Logger.WriteToLog("OnResume triggered", false);
            Logger.WriteToLog("OnResume | before Resume Recording called", false);
            try
            {
                ResumeRecordingAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Exception caused by ResumeRecordingAsync() " + ex.Source + " | " + ex.Message, false);
            }

            Logger.WriteToLog("OnResume | After Resume Recording called", false);
            Logger.WriteToLog("OnResume | Before GetCachedLocation called", false);
            try
            {
                await SpatialManager.GetCachedLocationAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Exception caused by GetCachedLocation(): {ex}", true);
            }
            Logger.WriteToLog("OnResume | After GetCachedLocation called", false);
        }


        protected async void OnResumeAsync()
        {
            
        }

        public static async void ResumeRecordingAsync()
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
