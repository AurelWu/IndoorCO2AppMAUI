

using IndoorCO2App_Android.Controls;
using static Android.Provider.CallLog;


namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private const string SelectedMonitorPreferenceKey = "SelectedMonitorIndex";
        public static int startTrimSliderValue = 0;
        public static int endTrimSliderValue = 1;

        public static bool startTrimSliderHasBeenUsed = false;
        public static bool endTrimSliderHasBeenUsed = false;
        public static bool endtrimSliderIsAtmax = false;

        int searchRange = 100;

        bool gpsActive = false;
        bool gpsGranted = false;
        bool btActive = false;
        bool btGranted = false;

        public bool hideLocation = true;
        public bool prerecording = false;

        public static int previousDataCount = 0;

        private readonly PeriodicTimer _timer;
        private DateTime timeOfLastGPSUpdate = DateTime.MinValue;

        public static bool hasOpenWindowsDoors;
        public static bool hasVentilationSystem;
        public static MainPage MainPageSingleton;
        internal CO2MonitorType monitorType;
        List<LocationData> locations;
        LocationData selectedLocation;

        bool firstInit = true;
        public bool manualRecordingMode = false;

        public MainPage()
        {
            InitializeComponent();
            CreateMainPageSingleton();
            InitUIElements();
            InitUILayout();
            ChangeToStandardUI();

            LoadMonitorType();
            BluetoothManager.Init();
            firstInit = false;

            UpdateUI();
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(0.4));
            Update();
        }

        private async void Update()
        {
            try
            {
                while (await _timer.WaitForNextTickAsync())
                {
                    UpdateUI();

                    //ENABLE AGAIN ONCE CORRECT
                    BluetoothManager.Update(monitorType);

                    if (DateTime.Now - timeOfLastGPSUpdate > TimeSpan.FromSeconds(15))
                    {
#if ANDROID
                        SpatialManager.UpdateLocation();
#endif
                        timeOfLastGPSUpdate = DateTime.Now;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Timer was stopped
            }
        }

        public void CreateMainPageSingleton()
        {
            if (MainPageSingleton == null)
            {
                MainPageSingleton = this;
            }
        }


        private void StartRecording(bool manualMode, bool resumedRecording)
        {
            BluetoothManager.recordedData = new List<SensorData>();
            this.manualRecordingMode = manualMode;
            _EndTrimSlider.Minimum = 0;
            _EndTrimSlider.Maximum = 1;
            _StartTrimSlider.Maximum = 1;
            startTrimSliderHasBeenUsed = false;
            endTrimSliderHasBeenUsed = false;
            previousDataCount = 0;
            //Console.WriteLine(LocationPicker);
            if (!manualMode)
            {
                if (!resumedRecording && _LocationPicker != null && _LocationPicker.SelectedItem != null && locations.Count > 0)
                {
                    selectedLocation = (LocationData)_LocationPicker.SelectedItem;
                    ChangeToRecordingUI();
                    long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    BluetoothManager.StartNewRecording(selectedLocation, startTime, prerecording);
                    RecoveryData.startTime = startTime;
                    RecoveryData.timeOfLastUpdate = startTime;
                    RecoveryData.locationID = selectedLocation.ID;
                    RecoveryData.locationType = selectedLocation.type;
                    RecoveryData.locationName = selectedLocation.Name;
                    RecoveryData.locationLat = selectedLocation.latitude;
                    RecoveryData.locationLon = selectedLocation.longitude;
                    RecoveryData.WriteToPreferences();

                }
                if (resumedRecording)
                {
                    LocationData ld = new LocationData(RecoveryData.locationType, RecoveryData.locationID, RecoveryData.locationName, RecoveryData.locationLat, RecoveryData.locationLon, RecoveryData.locationLat, RecoveryData.locationLon);
                    selectedLocation = ld;
                    ChangeToRecordingUI();
                    BluetoothManager.StartNewRecording(selectedLocation, RecoveryData.startTime, false);
                }


            }
            else if (manualMode)
            {
                ChangeToManualRecordingUI(); ;
                BluetoothManager.StartNewManualRecording(selectedLocation, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), prerecording);
            }
        }

        private void CancelRecording()
        {
            BluetoothManager.StopRecording();
            RecoveryData.ResetRecoveryData();
            ChangeToStandardUI();
        }


        public string GetNotesEditorText()
        {
            //FindByName avoids IDE Error in VS 2022 which doesn't understand that it is defined in XAML - change once that is fixed
            Editor ed = this.FindByName<Editor>("NotesEditor");
            if (ed.Text == null) return string.Empty;
            return ed.Text;
        }

        public void OnTransmissionFailed(string msg)
        {
           throw new System.NotImplementedException();
        }
        public void OnTransmissionSuccess(string msg)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateLocationPicker()
        {
#if ANDROID
            locations = OverpassModule.LocationData;
            if (locations.Count == 0)
            {
                return;
            }
            _LocationPicker.ItemsSource = null;
            _LocationPicker.Items.Clear();
            _LocationPicker.ItemsSource = locations;


            if (locations.Count > 0)
            {
                _LocationPicker.SelectedItem = locations[0];
            }
#endif
        }

        private void LoadMonitorType()
        {
            if (Preferences.ContainsKey(SelectedMonitorPreferenceKey))
            {
               int savedIndex = Preferences.Get(SelectedMonitorPreferenceKey, -1);
               if (savedIndex != -1 && savedIndex < _CO2DevicePicker.Items.Count)
               {
                   _CO2DevicePicker.SelectedIndex = savedIndex;
               
                   if (_CO2DevicePicker.SelectedItem.ToString() == "Aranet")
                   {
                       monitorType = CO2MonitorType.Aranet4;
                   }
                   else if (_CO2DevicePicker.SelectedItem.ToString() == "Airvalent")
                   {
                       monitorType = CO2MonitorType.Airvalent;
                   }
                   else if (_CO2DevicePicker.SelectedItem.ToString() == "Inkbird IAM-T1")
                   {
                       monitorType = CO2MonitorType.InkbirdIAMT1;
                   }
               }
            }
            else
            {
                _CO2DevicePicker.SelectedIndex = 0; // Set default selected item if no preference is stored
                monitorType = CO2MonitorType.Aranet4;
            }
        }
    }
}
