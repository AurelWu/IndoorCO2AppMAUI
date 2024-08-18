
using IndoorCO2App_Android.Controls;

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
        public Slider _EndTrimSlider;



        public void InitUIElements()
        {
            //FindByName avoids IDE Error in VS 2022 which doesn't understand that it is defined in XAML - change once that is fixed
            _GPSStatusButton = this.FindByName<ImageButton>("ButtonGPSStatus");
            _GPSPermissionButton = this.FindByName<ImageButton>("ButtonGPSPermission");
            _BluetoothEnabledButton = this.FindByName<ImageButton>("ButtonBluetoothStatus");
            _BluetoothPermissionsButton = this.FindByName<ImageButton>("ButtonBluetoothPermissions");
            _StatusLabel = this.FindByName<Label>("StatusLabel");
            _DeviceLabel = this.FindByName<Label>("DeviceLabel");
            _LocationLabel = this.FindByName<Label>("LocationLabel");
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
            _EndTrimSlider = this.FindByName<Slider>("endTrimSlider");


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
            MenuModesOfUIElements.Add(_ResumeRecordingButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(_StartRecordingButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(_StartManualRecordingButton, MenuMode.Standard);
            //MenuModesOfUIElements.Add(_FinishRecordingButton, MenuMode.Recording | MenuMode.ManualRecording); //stack is hidden so shouldnt be necessary?
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("PrerecordingLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(_OpenMapButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(_OpenImprintButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("DeleteLastSubmissionButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Label>("LocationLabelRecording"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("RecordingModeButtonStackLayout"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackManualName"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackManualAddress"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(_LineChartView, MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Label>("RecordedDataLabel"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("TrimSliderLayout"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Label>("TrimSliderInfoText"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackNotes"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("StackCheckboxesDoor"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("StackCheckboxesVentilation"), MenuMode.Recording | MenuMode.ManualRecording);
        }

        private void InitUILayout()
        {
            var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

            // Calculate 70% of the screen width
            var buttonWidth70Percent = screenWidth * 0.7;
            var buttonWidth60Percent = screenWidth * 0.6;
            var buttonWidth50Percent = screenWidth * 0.5;
            var buttonWidth30Percent = screenWidth * 0.3;
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
        }


        public void UpdateUI()
        {
            UpdateGPSStatusButton();
            UpdateGPSPermissionButton();
            UpdateBluetoothStatusButton();
            UpdateBluetoothPermissionsButton();

            UpdateLocationLabel();
        }

        public void UpdateGPSStatusButton()
        {
#if ANDROID
            var gpsActive = SpatialManager.CheckIfGpsIsEnabled();
            if (gpsActive)
            {
                _GPSStatusButton.BackgroundColor = Color.Parse("Green");
            }
            else
            {
                _GPSStatusButton.BackgroundColor = Color.Parse("Red");
            }
#endif
        }

        private async void UpdateGPSPermissionButton()
        {
#if ANDROID
            gpsGranted = await SpatialManager.IsLocationPermissionGrantedAsync();
            if (gpsGranted)
            {
                _GPSPermissionButton.BackgroundColor = Color.Parse("Green");
            }
            else
            {
                _GPSPermissionButton.BackgroundColor = Color.Parse("Red");
            }
#endif
        }

        private void UpdateBluetoothStatusButton()
        {
        #if ANDROID
            btActive = BluetoothHelper.CheckIfBTEnabled();
            if (btActive)
            {
                _BluetoothEnabledButton.BackgroundColor = Color.Parse("Green");
            }
            else
            {
                _BluetoothEnabledButton.BackgroundColor = Color.Parse("Red");
            }
        #endif
        }

        private void UpdateBluetoothPermissionsButton()
        {
#if ANDROID
            btGranted = BluetoothHelper.CheckStatus();
            if (btGranted)
            {
                _BluetoothPermissionsButton.BackgroundColor = Color.Parse("Green");
            }
            else
            {
                _BluetoothPermissionsButton.BackgroundColor = Color.Parse("Red");
            }
#endif
        }

        private void UpdateLocationLabel()
        {
            if (gpsActive && gpsGranted)
            {
                if (SpatialManager.currentLocation.Latitude != 0 || SpatialManager.currentLocation.Longitude != 0)
                {
                    if (!hideLocation)
                    {
                        LocationLabel.Text = "Lat: " + SpatialManager.currentLocation.Latitude.ToString("0.######") + " | Lon:" + SpatialManager.currentLocation.Longitude.ToString("0.######") + " (tap to hide)";
                    }
                    else
                    {
                        LocationLabel.Text = "Lat: " + SpatialManager.currentLocation.Latitude.ToString("0.#") + "****" + " | Lon:" + SpatialManager.currentLocation.Longitude.ToString("0.#" + "****") + " (tap to show)";
                    }
                }
                else
                {
                    LocationLabel.Text = "GPS enabled and Location permissions granted. Getting first Location Info. This might take a minute";
                }
            }
            else
            {
                LocationLabel.Text = ("");
                if (!gpsActive) LocationLabel.Text += ("GPS not enabled ");
                if (!gpsGranted) LocationLabel.Text += ("Location Permission missing");
            }
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
