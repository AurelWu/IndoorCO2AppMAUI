
using IndoorCO2App_Android.Controls;
using Syncfusion.Maui.Sliders;

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        Dictionary<VisualElement, MenuMode> MenuModesOfUIElements;

        public ImageButton _GPSPermissionButton;
        public ImageButton _GPSStatusButton;
        public ImageButton _BluetoothEnabledButton;
        public ImageButton _BluetoothPermissionsButton;
        public Label _StatusLabel;
        public Label _DeviceLabel;
        public Label _LocationLabel;
        public Label _LocationLabelRecording;
        public Label _LocationInfoLabel;
        public Button _UpdateLocationsButton;
        public Button _ResumeRecordingButton;
        public Button _StartRecordingButton;
        public Button _FinishRecordingButton;
        public Button _StartManualRecordingButton;
        public LineChartView _LineChartView;
        public Picker _CO2DevicePicker;
        public Picker _LocationPicker;
        public RadioButton _RadioButton50m;
        public RadioButton _RadioButton100m;
        public RadioButton _RadioButton250m;
        public Button _OpenMapButton;
        public Button _OpenImprintButton;
        public Button _DeleteLastSubmissionButton;
        public Button _RequestCancelRecordingButton;
        public Button _ConfirmCancelRecordingButton;
        public CheckBox _CheckBoxDoorsWindows;
        public CheckBox _CheckBoxVentilation;
        public Switch _PrerecordingSwitch;
        //public Slider _EndTrimSlider;
        //public Slider _StartTrimSlider;
        public Editor _ManualNameEditor;
        public Editor _ManualAddressEditor;
        public SfRangeSlider _TrimSlider;
        public Editor _CO2DeviceNameFilterEditor;




        public void InitUIElements()
        {
            //FindByName avoids IDE Error in VS 2022 which doesn't understand that it is defined in XAML - change once that is fixed
            _TrimSlider = this.FindByName<SfRangeSlider>("TrimSlider");
            _GPSStatusButton = this.FindByName<ImageButton>("ButtonGPSStatus");
            _GPSPermissionButton = this.FindByName<ImageButton>("ButtonGPSPermission");
            _BluetoothEnabledButton = this.FindByName<ImageButton>("ButtonBluetoothStatus");
            _BluetoothPermissionsButton = this.FindByName<ImageButton>("ButtonBluetoothPermissions");
            _StatusLabel = this.FindByName<Label>("StatusLabel");
            _DeviceLabel = this.FindByName<Label>("DeviceLabel");
            _LocationLabel = this.FindByName<Label>("LocationLabel");
            _LocationLabelRecording = this.FindByName<Label>("LocationLabelRecording");
            _LocationInfoLabel = this.FindByName<Label>("LocationInfoLabel");
            _UpdateLocationsButton = this.FindByName<Button>("UpdateLocationsButton");
            _ResumeRecordingButton = this.FindByName<Button>("ResumeRecordingButton");
            _StartRecordingButton = this.FindByName<Button>("StartRecordingButton");
            _StartManualRecordingButton = this.FindByName<Button>("StartManualRecordingButton");
            _LineChartView = this.FindByName<LineChartView>("lineChartView");
            _CO2DevicePicker = this.FindByName<Picker>("CO2MonitorPicker");
            _LocationPicker = this.FindByName<Picker>("LocationPicker");
            _RadioButton50m = this.FindByName<RadioButton>("RadioButton50m");
            _RadioButton100m = this.FindByName<RadioButton>("RadioButton100m");
            _RadioButton250m = this.FindByName<RadioButton>("RadioButton250m");
            _OpenMapButton = this.FindByName<Button>("OpenMapButton");
            _OpenImprintButton = this.FindByName<Button>("OpenImprintButton");
            _DeleteLastSubmissionButton = this.FindByName<Button>("DeleteLastSubmissionButton");
            _FinishRecordingButton = this.FindByName<Button>("FinishRecordingButton");
            _RequestCancelRecordingButton = this.FindByName<Button>("RequestCancelRecordingButton");
            _ConfirmCancelRecordingButton = this.FindByName<Button>("ConfirmCancelRecordingButton");
            _CheckBoxDoorsWindows = this.FindByName<CheckBox>("CheckBoxDoorsWindows");
            _CheckBoxVentilation = this.FindByName<CheckBox>("CheckBoxVentilation");
            _PrerecordingSwitch = this.FindByName<Switch>("PrerecordingSwitch");
            //_EndTrimSlider = this.FindByName<Slider>("endTrimSlider");
            //_StartTrimSlider = this.FindByName<Slider>("startTrimSlider");
            _ManualAddressEditor = this.FindByName<Editor>("ManualAddressEditor");
            _ManualNameEditor = this.FindByName<Editor>("ManualNameEditor");
            _ConfirmCancelRecordingButton.IsVisible = false;
            _CO2DeviceNameFilterEditor = this.FindByName<Editor>("CO2DeviceNameFilterEditor");
            _ResumeRecordingButton.IsVisible = false; //TODO Enable again once completely implemented


            MenuModesOfUIElements = new Dictionary<VisualElement, MenuMode>();
            MenuModesOfUIElements.Add(_GPSStatusButton, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(_GPSPermissionButton, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(_BluetoothEnabledButton, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(_BluetoothPermissionsButton, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("CO2MonitorPickerStackLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(_StatusLabel, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(_DeviceLabel, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(_LocationLabel, MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("SearchRangeStackLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(_UpdateLocationsButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<VerticalStackLayout>("LocationStackLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(_ResumeRecordingButton, 0); 
            MenuModesOfUIElements.Add(_StartRecordingButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(_StartManualRecordingButton, MenuMode.Standard);
            //MenuModesOfUIElements.Add(_FinishRecordingButton, MenuMode.Recording | MenuMode.ManualRecording); //stack is hidden so shouldnt be necessary?
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("PrerecordingLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(_OpenMapButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(_OpenImprintButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("DeleteLastSubmissionButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(_LocationLabelRecording, MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(_ConfirmCancelRecordingButton, 0);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("RecordingModeButtonStackLayout"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackManualName"), MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackManualAddress"), MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(_LineChartView, MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Label>("RecordedDataLabel"), MenuMode.Recording | MenuMode.ManualRecording);
            //MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("TrimSliderLayout"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Label>("TrimSliderInfoText"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackNotes"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("StackCheckboxesDoor"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("StackCheckboxesVentilation"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackDeviceNameFilter"), MenuMode.Standard);
            MenuModesOfUIElements.Add(_TrimSlider, MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(_CO2DeviceNameFilterEditor, MenuMode.Standard);
        }

        private void InitUILayout()
        {
            var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
            // Calculate 70% of the screen width
            var buttonWidth70Percent = screenWidth * 0.70;
            var buttonWidth60Percent = screenWidth * 0.60;
            var buttonWidth50Percent = screenWidth * 0.50;
            var buttonWidth30Percent = screenWidth * 0.30;
            var buttonWidth25Percent = screenWidth * 0.25;

            // Set the button's minimum width
            _ResumeRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
            _StartRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
            _StartManualRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
            _UpdateLocationsButton.MinimumWidthRequest = buttonWidth70Percent;
            _OpenMapButton.MinimumWidthRequest = buttonWidth70Percent;
            _OpenImprintButton.MinimumWidthRequest = buttonWidth70Percent;
            _FinishRecordingButton.MinimumWidthRequest = buttonWidth60Percent;
            _FinishRecordingButton.MaximumWidthRequest = buttonWidth60Percent;
            _RequestCancelRecordingButton.MinimumWidthRequest = buttonWidth25Percent;
            _RequestCancelRecordingButton.MaximumWidthRequest = buttonWidth25Percent;
            _ConfirmCancelRecordingButton.MinimumWidthRequest = buttonWidth25Percent;
            _ConfirmCancelRecordingButton.MaximumWidthRequest = buttonWidth25Percent;
            _RadioButton100m.IsChecked = true;
            _DeleteLastSubmissionButton.MinimumWidthRequest = buttonWidth70Percent;
            _LineChartView.MinimumHeightRequest = screenHeight * 0.20;
            _TrimSlider.WidthRequest = _LineChartView.Width;
        }


        public void UpdateUI()
        {
            UpdateGPSStatusButton();
            UpdateGPSPermissionButton();
            UpdateBluetoothStatusButton();
            UpdateBluetoothPermissionsButton();

            UpdateLocationLabel();
            UpdateLocationRecordingLabel();
            UpdateLocationInfoLabel();

            UpdateLineChart();

            UpdateStatusLabel();
            UpdateDeviceLabel();

            UpdateStartRecordingButton();
            UpdateFinishRecordingButton();
        }

        private void UpdateStartRecordingButton()
        {
            if (gpsActive && gpsGranted && btGranted && btActive && OverpassModule.LocationData.Count > 0 && BluetoothManager.discoveredDevices != null && BluetoothManager.discoveredDevices.Count > 0 && BluetoothManager.currentCO2Reading > 0)
            {
                _StartRecordingButton.IsEnabled = true;
            }
            else
            {
                _StartRecordingButton.IsEnabled = false;

            }

            if (gpsActive && gpsGranted && btGranted && btActive && OverpassModule.everFetchedLocations == true && OverpassModule.currentlyFetching == false && BluetoothManager.discoveredDevices != null && BluetoothManager.discoveredDevices.Count > 0 && BluetoothManager.currentCO2Reading > 0)
            {
                _StartManualRecordingButton.IsEnabled = true;
            }
            else
            {
                _StartManualRecordingButton.IsEnabled = false;
            }
        }

        private void UpdateFinishRecordingButton()
        {
            int original = BluetoothManager.recordedData.Count;
            int trimStart = (int)Math.Floor(_TrimSlider.RangeStart);
            int trimEnd = (int)Math.Floor(_TrimSlider.RangeEnd);
            if (trimEnd - trimStart >= 4 && BluetoothManager.isRecording)
            {
                if (manualRecordingMode && (_ManualNameEditor.Text.Length < 1 || _ManualAddressEditor.Text.Length < 1))
                {
                    _FinishRecordingButton.IsEnabled = false;
                    _FinishRecordingButton.Text = "Submit Data (needs Address & Name)";
                }
                else
                {
                    _FinishRecordingButton.IsEnabled = true;
                    _FinishRecordingButton.Text = "Submit Data";
                }
            }
            else if (BluetoothManager.isRecording)
            {
                _FinishRecordingButton.IsEnabled = false;
                _FinishRecordingButton.Text = "Submit Data (needs 5 Minutes of Data)";
            }


        }

        private void UpdateDeviceLabel()
        {
            try
            {
                if (!btGranted || !btActive)
                {
                    _DeviceLabel.Text = "Bluetooth not enabled or permissions missing, can not fetch Sensor Data";
                }
                else if (BluetoothManager.discoveredDevices.Count == 0)
                {
                    if(_CO2DeviceNameFilterEditor.Text != null && _CO2DeviceNameFilterEditor.Text.Length > 0)
                    {
                        _DeviceLabel.Text = $"Device not yet found. This might take a while. Namefilter set to: {_CO2DeviceNameFilterEditor.Text}";
                    }
                    else
                    {
                        _DeviceLabel.Text = $"Device not yet found. This might take a while";
                    }
                    

                    if (BluetoothManager.lastAttemptFailed)
                    {
                        _DeviceLabel.Text += " | previous update failed";
                    }
                }
                else if (BluetoothManager.sensorUpdateInterval > 60)
                {
                    _DeviceLabel.Text = "Device found but Update Interval not set to 1 Minute, change to 1 Minute using official App. next attempt in " + BluetoothManager.timeToNextUpdate + "s";
                }
                else if (BluetoothManager.currentCO2Reading != 0 && BluetoothManager.gattStatus == 0) //TODO also add check if last reading was a success maybe?         
                {
                    _DeviceLabel.Text = "CO2 Levels: " + BluetoothManager.currentCO2Reading + " |  Update in: " + BluetoothManager.timeToNextUpdate + "s" + "\r\n | rssi: " + BluetoothManager.rssi + " | Gatt Status: " + BluetoothManager.gattStatus;
                    if (BluetoothManager.lastAttemptFailed)
                    {
                        _DeviceLabel.Text += " | previous update failed";
                    }
                }
                else if (BluetoothManager.currentCO2Reading == 0 && BluetoothManager.isGattA2DP == true)
                {
                    _DeviceLabel.Text = "Sensor found, but the required 'Smart Home Integration' is disabled.\r\n Please enable it using the official Aranet App (use the Gears Icon)";
                }
                else if (BluetoothManager.currentCO2Reading == 0)
                {
                    _DeviceLabel.Text = "initiating first Update in:" + BluetoothManager.timeToNextUpdate + "s" + "\r\n | rssi: " + BluetoothManager.rssi + " | Gatt Status: " + BluetoothManager.gattStatus;
                    if (BluetoothManager.lastAttemptFailed)
                    {
                        _DeviceLabel.Text += " | previous update failed";
                    }
                }
            }
            catch
            {
                _DeviceLabel.Text = "update failed - next attempt in: " + BluetoothManager.timeToNextUpdate;
                //Debug.WriteLine("UpdateDeviceLabel - exception caught");
            }


        }

        private void UpdateStatusLabel()
        {
            {
                _StatusLabel.Text = ("");
                if (!gpsActive) _StatusLabel.Text += "GPS not enabled | ";
                if (!gpsGranted) _StatusLabel.Text += "Location Permission missing |";
                if (!btActive) _StatusLabel.Text += "Bluetooth not enabled |";
                if (!btGranted) _StatusLabel.Text += "Bluetooth permission not granted";
                if (gpsActive && gpsGranted && btActive && btGranted) _StatusLabel.Text = "GPS & Bluetooth Permissions and Status okay";
            }
        }

        public void UpdateGPSStatusButton()
        {

            gpsActive = SpatialManager.CheckIfGpsIsEnabled();
            if (gpsActive)
            {
                _GPSStatusButton.BackgroundColor = Color.Parse("Green");
            }
            else
            {
                _GPSStatusButton.BackgroundColor = Color.Parse("Red");
            }

        }

        private async void UpdateGPSPermissionButton()
        {

            gpsGranted = await SpatialManager.IsLocationPermissionGrantedAsync();
            if (gpsGranted)
            {
                _GPSPermissionButton.BackgroundColor = Color.Parse("Green");
            }
            else
            {
                _GPSPermissionButton.BackgroundColor = Color.Parse("Red");
            }

        }

        private void UpdateBluetoothStatusButton()
        {

            btActive = BluetoothHelper.CheckIfBTEnabled();
            if (btActive)
            {
                _BluetoothEnabledButton.BackgroundColor = Color.Parse("Green");
            }
            else
            {
                _BluetoothEnabledButton.BackgroundColor = Color.Parse("Red");
            }

        }

        private void UpdateBluetoothPermissionsButton()
        {

            btGranted = BluetoothHelper.CheckStatus();
            if (btGranted)
            {
                _BluetoothPermissionsButton.BackgroundColor = Color.Parse("Green");
            }
            else
            {
                _BluetoothPermissionsButton.BackgroundColor = Color.Parse("Red");
            }

        }

        private void UpdateLocationLabel()
        {
            if (gpsActive && gpsGranted)
            {
                if (SpatialManager.currentLocation.Latitude != 0 || SpatialManager.currentLocation.Longitude != 0)
                {
                    if (!hideLocation)
                    {
                        _LocationLabel.Text = "Lat: " + SpatialManager.currentLocation.Latitude.ToString("0.######") + " | Lon:" + SpatialManager.currentLocation.Longitude.ToString("0.######") + " (tap to hide)";
                    }
                    else
                    {
                        _LocationLabel.Text = "Lat: " + SpatialManager.currentLocation.Latitude.ToString("0.#") + "****" + " | Lon:" + SpatialManager.currentLocation.Longitude.ToString("0.#" + "****") + " (tap to show)";
                    }
                }
                else
                {
                    _LocationLabel.Text = "GPS enabled and Location permissions granted. Getting first Location Info. This might take a minute";
                }
            }
            else
            {
                _LocationLabel.Text = ("");
                if (!gpsActive) _LocationLabel.Text += ("GPS not enabled ");
                if (!gpsGranted) _LocationLabel.Text += ("Location Permission missing");
            }
        }

        private void UpdateLocationRecordingLabel()
        {
            if (selectedLocation != null)
            {
                _LocationLabelRecording.Text = "Location: " + selectedLocation.Name;
            }
            else
            {
                _LocationLabelRecording.Text = "No Location Selected";
            }
        }

        private void UpdateLocationInfoLabel()
        {
            if (OverpassModule.everFetchedLocations == false)
            {
                _LocationInfoLabel.Text = "Press Update Locations to get nearby locations";
            }
            else if (OverpassModule.lastFetchWasSuccessButNoResults)
            {
                _LocationInfoLabel.Text = "No locations in range";
            }
            else if (OverpassModule.lastFetchWasSuccess)
            {
                _LocationInfoLabel.Text = "Select Location:";
            }
            else if (OverpassModule.lastFetchWasSuccess == false && !OverpassModule.currentlyFetching)
            {
                _LocationInfoLabel.Text = "Update locations request failed, try again later";
            }
            else if (OverpassModule.currentlyFetching)
            {
                _LocationInfoLabel.Text = "currently retrieving nearby locations";
            }
        }

        private void UpdateLineChart()
        {
            //TODO: RangeSliderMax is 1 when 2 elements ( 0, 1) but logic is based on 0,2 in some places still
            _LineChartView.SetData(BluetoothManager.recordedData);
            int maxSliderVal = BluetoothManager.recordedData.Count - 1;
            if (maxSliderVal < 0) maxSliderVal = 0;
            _TrimSlider.Minimum = 0;
            _TrimSlider.Maximum = maxSliderVal;


            if (_TrimSlider.Maximum == 0)
            {
                _TrimSlider.Maximum = 1;
                _TrimSlider.RangeStart = 0;
                _TrimSlider.RangeEnd = 1;
            }
            else if (maxSliderVal > 0)
            {
                _TrimSlider.Maximum = maxSliderVal;

            }

            if (maxSliderVal > previousDataCount)
            {
                int max = Math.Max(1, maxSliderVal);
                previousDataCount = maxSliderVal;
                if (_TrimSlider.RangeEnd == max - 1) _TrimSlider.RangeEnd = max;
            }

            if (!endTrimSliderHasBeenUsed && maxSliderVal > 0)
            {
                _TrimSlider.RangeEnd = _TrimSlider.Maximum;
            }
            //if (endtrimSliderIsAtmax)
            //{
            //_TrimSlider.RangeEnd = _TrimSlider.Maximum;
            //}

            if (_TrimSlider.RangeEnd == _TrimSlider.Maximum)
            {
                endtrimSliderIsAtmax = true;
            }
            else
            {
                endtrimSliderIsAtmax = false;
            }

            startTrimSliderValue = (int)_TrimSlider.RangeStart;
            endTrimSliderValue = (int)_TrimSlider.RangeEnd;
        }


        private void ChangeToUI(MenuMode mode)
        {
            foreach (var element in MenuModesOfUIElements)
            {
                var k = element.Key;
                var flags = element.Value;
                if (flags.HasFlag(mode))
                {
                    k.IsVisible = true;
                }
                else
                {
                    k.IsVisible = false;
                }
            }
        }

        public void ChangeToStandardUI()
        {
            ChangeToUI(MenuMode.Standard);
            if (RecoveryData.locationID != 0)
            {
                _ResumeRecordingButton.IsVisible = true;
            }
            else
            {
                _ResumeRecordingButton.IsVisible = false;
            }
        }    

        public void ChangeToRecordingUI()
        {
            ChangeToUI(MenuMode.Recording);
        }

        public void ChangeToManualRecordingUI()
        {
            ChangeToUI(MenuMode.ManualRecording);
        }
    }

}
