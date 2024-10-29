
using IndoorCO2App_Android;
using IndoorCO2App_Multiplatform.Controls;
using System.ComponentModel;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private string _appVersion;
        public event PropertyChangedEventHandler PropertyChanged;
        public string AppVersion
        {
            get => _appVersion;
            set
            {
                if (_appVersion != value)
                {
                    _appVersion = value;
                    OnPropertyChanged(nameof(AppVersion)); // Notify the UI that the property changed
                }
            }
        }

        private const string SelectedMonitorPreferenceKey = "SelectedMonitorIndex";
        private const string DeviceNameFilterPreferenceKey = "NameFilterValue";
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
        List<LocationData> transitOriginLocations;
        List<LocationData> transitTargetLocations;
        List<TransitLineData> transitLines;
        internal LocationData selectedLocation;
        internal LocationData selectedTransitOriginLocation;
        internal LocationData selectedTransitTargetLocation;
        internal TransitLineData selectedTransitLine;

        bool firstInit = true;
        public SubmissionMode submissionMode;
        //public bool transportRecordingMode = false;
        public IBluetoothHelper bluetoothHelper;

        public MainPage()
        {
            InitializeComponent();
            AppVersion = GetAppVersion();
            CreateMainPageSingleton();
            InitUIElements();
            InitUILayout();
            RecoveryData.ReadFromPreferences();
            _CO2DeviceNameFilterEditor.Text = Preferences.Get(DeviceNameFilterPreferenceKey, "");
            ChangeToStandardUI();

            LoadMonitorType();
#if ANDROID
            bluetoothHelper = new BluetoothHelper();            
#endif
#if IOS
            bluetoothHelper = new BluetoothHelperApple();
//TODO: add bluetoothHelper for iPhone
#endif
            //TODO: Add iPhone BluetoothHelper Implementation
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

                    BluetoothManager.Update(monitorType,_CO2DeviceNameFilterEditor.Text,bluetoothHelper);

                    if (DateTime.Now - timeOfLastGPSUpdate > TimeSpan.FromSeconds(15))
                    {
                        SpatialManager.UpdateLocation();

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

        public void BluetoothHelperSingleton()
        {
            
        }


        private void StartRecording(SubmissionMode submissionMode, bool resumedRecording)
        {
            this.submissionMode = submissionMode;
            BluetoothManager.recordedData = new List<SensorData>();
            _TrimSlider.Minimum = 0;
            _TrimSlider.Maximum = 1;            
            startTrimSliderHasBeenUsed = false;
            endTrimSliderHasBeenUsed = false;
            previousDataCount = 0;
            //Console.WriteLine(LocationPicker);
            if (submissionMode== SubmissionMode.Building)
            {
                if (!resumedRecording && _LocationPicker != null && _LocationPicker.SelectedItem != null && locations.Count > 0)
                {
                    selectedLocation = (LocationData)_LocationPicker.SelectedItem;
                    ChangeToRecordingUI();
                    long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    BluetoothManager.StartNewRecording(monitorType, selectedLocation, startTime, prerecording);
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
                    BluetoothManager.StartNewRecording(monitorType, selectedLocation, RecoveryData.startTime, false);
                }


            }
            else if (submissionMode== SubmissionMode.BuildingManual)
            {
                ChangeToManualRecordingUI(); ;
                BluetoothManager.StartNewManualRecording(selectedLocation, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), prerecording);
            }            
        }

        private void StartTransportRecording()
        {
            submissionMode = SubmissionMode.Transit;
            selectedTransitOriginLocation = (LocationData)_TransitOriginPicker.SelectedItem;
            selectedTransitLine = (TransitLineData) _TransitLinePicker.SelectedItem;
            BluetoothManager.recordedData = new List<SensorData>();            
            _TrimSlider.Minimum = 0;
            _TrimSlider.Maximum = 1;
            startTrimSliderHasBeenUsed = false;
            endTrimSliderHasBeenUsed = false;
            previousDataCount = 0;
            long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ChangeToTransportRecordingUI();
            BluetoothManager.StartTransportRecording(monitorType,startTime, prerecording,selectedTransitOriginLocation,selectedTransitLine);
        }

        private void CancelRecording()
        {
            ResetRecordingState();
            BluetoothManager.StopRecording();
            RecoveryData.ResetRecoveryData();
            ChangeToStandardUI();
        }

        private void ResetRecordingState()
        {
            SpatialManager.ResetLocation();
            _LocationPicker.ItemsSource = null;
            _LocationPicker.Items.Clear();
            _TransitDestinationPicker.ItemsSource = null;
            _TransitDestinationPicker.Items.Clear();
            _TransitOriginPicker.ItemsSource = null;
            _TransitOriginPicker.Items.Clear();
            _TransitLinePicker.ItemsSource = null;
            _TransitLinePicker.Items.Clear();
            OverpassModule.TransitLines.Clear();
            OverpassModule.TransportStartLocationData.Clear();
            OverpassModule.TransportDestinationLocationData.Clear();
            _CheckBoxDoorsWindows.IsChecked = false;
            _CheckBoxVentilation.IsChecked = false;                        
        }
        private void ResetNotes()
        {
            Editor ed = this.FindByName<Editor>("NotesEditor");
            ed.Text = "";
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
            _FinishRecordingButton.Text = msg;
            _FinishRecordingButton.IsEnabled = true;
        }
        public void OnTransmissionSuccess(string msg)
        {
            _FinishRecordingButton.Text = "Transmission successful!";
            OverpassModule.lastFetchWasSuccess = false;
            OverpassModule.lastFetchWasSuccessButNoResults = false;
            OverpassModule.everFetchedLocations = false;
            ResetNotes();

            Application.Current.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(4), () =>
            {
                RecoveryData.ResetRecoveryData();
                ChangeToUI(MenuMode.Standard);
            });
        }

        public void UpdateLocationPicker()
        {

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
        }

        public void UpdateTransitOriginPicker()
        {
            transitOriginLocations = OverpassModule.TransportStartLocationData;
            if(transitOriginLocations.Count == 0)
            {
                return;
            }
            _TransitOriginPicker.ItemsSource = null;
            _TransitOriginPicker.Items.Clear();
            _TransitOriginPicker.ItemsSource = transitOriginLocations;
            if(transitOriginLocations.Count > 0)
            {
                _TransitOriginPicker.SelectedItem =transitOriginLocations[0];
            }
            //TODO create TransitOriginLocationPicker and assign stuff here
        }

        public void UpdateTransitDestinationPicker()
        {
            transitTargetLocations = OverpassModule.TransportDestinationLocationData;
            if (transitTargetLocations.Count == 0)
            {
                return;
            }
            _TransitDestinationPicker.ItemsSource = null;
            _TransitDestinationPicker.Items.Clear();
            _TransitDestinationPicker.ItemsSource = transitTargetLocations;
            if (transitOriginLocations.Count > 0)
            {
                _TransitDestinationPicker.SelectedItem = transitTargetLocations[0];
            }
            //TODO create TransitDestinationLocationPicker and assign stuff here
        }

        public void UpdateTransitLinesPicker()
        {
            transitLines = OverpassModule.TransitLines;
            if(transitLines.Count == 0)
            {
                return;
            }
            _TransitLinePicker.ItemsSource = null;
            _TransitLinePicker.Items.Clear();
            _TransitLinePicker.ItemsSource= transitLines;
            if (transitLines.Count > 0)
            {
                _TransitLinePicker.SelectedItem = transitLines[0];
            }
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

        private string GetAppVersion()
        {
            // Retrieve the app version
            return "Version: " + VersionTracking.CurrentVersion;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task<bool> DisplayTransitSubmissionNoDestinationConfirmationDialog()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Submit without destination?", // Title
                "Are you sure you want to submit without a destination?", // Message
                "Confirm", // Confirm button
                "Cancel" // Cancel button
            );

            return answer; // Returns true if 'Confirm' is pressed, false if 'Cancel' is pressed
        }
    }
}
