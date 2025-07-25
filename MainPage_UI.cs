﻿
using IndoorCO2App_Multiplatform.Controls;
using Syncfusion.Maui.Sliders;
using Mapsui.UI.Maui;
using Mapsui;
using CommunityToolkit.Maui.Views;
using Microsoft.VisualStudio.RpcContracts.Commands;
using IndoorCO2App_Android;
using Microsoft.Maui.Controls.Platform;


namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        Dictionary<VisualElement, MenuMode> MenuModesOfUIElements;

        public Label _FilterDeviceByNameLabel;
        public Label _OpenWindowsLabel;
        public Label _VentilationLabel;
        public Label _TrimSliderInfoText;
        public Label _PrerecordingLabel;
        public FixedScrollView _MainScrollView;
        public ImageButton _GPSPermissionButton;
        public ImageButton _GPSStatusButton;
        public ImageButton _BluetoothEnabledButton;
        public ImageButton _BluetoothPermissionsButton;
        public Label _SearchRange;
        public Label _StatusLabel;
        public Label _DeviceLabel;
        public Label _DevicePickerLabel;
        public Label _LocationLabel;
        public Label _LocationLabelRecording;
        public Label _LocationInfoLabel;
        public Label _VersionLabel;
        public Button _UpdateLocationsButton;
        public Button _GetCachedLocationsButton;
        public Button _ResumeRecordingButton;
        public Button _StartRecordingButton;
        public Button _FinishRecordingButton;
        public Button _StartManualRecordingButton;
        public Button _StartTransportRecordingButton;
        public LineChartView _LineChartView;
        public Picker _CO2DevicePicker;
        public Picker _LocationPicker;
        public Picker _TransitOriginPicker;
        public Picker _TransitDestinationPicker;
        public Picker _TransitLinePicker;
        //public RadioButton _RadioButton50m;
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
        public Entry _CO2DeviceNameFilterEditor;
        public Button _DebugLogButton;
        public Button _CrashLogButton;
        public Button _BuildingModeButton;
        public Button _TransitModeButton;
        public Button _TransitFilterAllButton;
        public Button _TransitFilterBusButton;
        public Button _TransitFilterTramButton;
        public Button _TransitFilterSubwayButton;
        public Button _TransitFilterLightRailButton;
        public Button _TransitFilterTrainButton;

        public ImageButton _StarIconToggleBuilding;
        public ImageButton _StarIconToggleTransitLine;
        public ImageButton _StarIconToggleTransitOrigin;
        public ImageButton _StarIconToggleTransitDestination;

        public Entry _TransitLineSearchFilterEditor;

        internal MenuMode currentMenuMode;

        public MapView _mapView;
        public Expander _mapViewExpander;
        public Label _mapViewExpanderLabel;
        public Label _SuccessNotificationLabel;

        public HorizontalStackLayout _StatusButtons;
        public HorizontalStackLayout _ButtonBuildingTransitSelectionStackLayout;

        public Editor _NotesEditor;

        public Grid _LocationUpdateGrid;

        public Button _QuickGuideButtonTop;
        public Button _QuickGuideButtonBottom;
        LocalisationResourceManager loc;


        public void InitUIElements()
        {
            _FilterDeviceByNameLabel = this.FindByName<Label>("FilterDeviceByNameLabel");
            _OpenWindowsLabel = this.FindByName<Label>("OpenWindowsLabel");
            _VentilationLabel = this.FindByName<Label>("VentilationLabel");
            _TrimSliderInfoText = this.FindByName<Label>("TrimSliderInfoText");
            _PrerecordingLabel = this.FindByName<Label>("PrerecordingLabel");
            _SearchRange = this.FindByName<Label>("LabelSearchRange");
            _QuickGuideButtonTop = this.FindByName<Button>("QuickGuideButtonTop");
            _QuickGuideButtonBottom = this.FindByName<Button>("QuickGuideButtonBottom");
            _LocationUpdateGrid = this.FindByName<Grid>("LocationUpdateGrid");
            _NotesEditor = this.FindByName<Editor>("NotesEditor");
            _ButtonBuildingTransitSelectionStackLayout = this.FindByName<HorizontalStackLayout>("ButtonBuildingTransitSelectionStackLayout");
            _StatusButtons = this.FindByName<HorizontalStackLayout>("StatusButtons");
            //FindByName avoids IDE Error in VS 2022 which doesn't understand that it is defined in XAML - change once that is fixed
            _MainScrollView = this.FindByName<FixedScrollView>("MainScrollView");
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
            _VersionLabel = this.FindByName<Label>("VersionLabel");
            _UpdateLocationsButton = this.FindByName<Button>("UpdateLocationsButton");
            _GetCachedLocationsButton = this.FindByName<Button>("GetCachedLocationsButton");
            _ResumeRecordingButton = this.FindByName<Button>("ResumeRecordingButton");
            _StartRecordingButton = this.FindByName<Button>("StartRecordingButton");
            _StartManualRecordingButton = this.FindByName<Button>("StartManualRecordingButton");
            _StartTransportRecordingButton = this.FindByName<Button>("StartTransportRecordingButton");

            _LineChartView = this.FindByName<LineChartView>("lineChartView");
            _CO2DevicePicker = this.FindByName<Picker>("CO2MonitorPicker");
            _LocationPicker = this.FindByName<Picker>("LocationPicker");
            _TransitOriginPicker = this.FindByName<Picker>("TransitOriginPicker");
            _TransitDestinationPicker = this.FindByName<Picker>("TransitDestinationPicker");
            _TransitLinePicker = this.FindByName<Picker>("TransitLinePicker");

            //_RadioButton50m = this.FindByName<RadioButton>("RadioButton50m");
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
            _CO2DeviceNameFilterEditor = this.FindByName<Entry>("CO2DeviceNameFilterEditor");
            _ResumeRecordingButton.IsVisible = false; //TODO Enable again once completely implemented
            _DebugLogButton = this.FindByName<Button>("DebugLogButton");
            _CrashLogButton = this.FindByName<Button>("CrashLogButton");

            _BuildingModeButton = this.FindByName<Button>("ButtonBuildingMode");
            _TransitModeButton = this.FindByName<Button>("ButtonTransitMode");
            _TransitFilterAllButton = this.FindByName<Button>("ButtonAll");
            _TransitFilterBusButton = this.FindByName<Button>("ButtonBus");
            _TransitFilterTramButton = this.FindByName<Button>("ButtonTram");
            _TransitFilterSubwayButton = this.FindByName<Button>("ButtonSubway");
            _TransitFilterLightRailButton = this.FindByName<Button>("ButtonLightRail");
            _TransitFilterTrainButton = this.FindByName<Button>("ButtonTrain");
            _StarIconToggleBuilding = this.FindByName<ImageButton>("StarIconToggleBuilding");
            _StarIconToggleTransitLine = this.FindByName<ImageButton>("StarIconToggleTransitLine");
            _StarIconToggleTransitOrigin = this.FindByName<ImageButton>("StarIconToggleTransitOrigin");
            _StarIconToggleTransitDestination = this.FindByName<ImageButton>("StarIconToggleTransitDestination");

            _TransitLineSearchFilterEditor = this.FindByName<Entry>("TransitLineSearchFilterEditor");
            _mapView = this.FindByName<MapView>("mapView");
            _mapViewExpander = this.FindByName<Expander>("mapViewExpander");
            _mapViewExpanderLabel = this.FindByName<Label>("mapViewExpanderLabel");
            _SuccessNotificationLabel = this.FindByName<Label>("SuccessNotificationLabel");
            _DevicePickerLabel = this.FindByName<Label>("DevicePickerLabel");



            MenuModesOfUIElements = new Dictionary<VisualElement, MenuMode>();
            
            //MenuModesOfUIElements.Add(_DevicePickerLabel, MenuMode.Standard);
            MenuModesOfUIElements.Add(_GPSStatusButton, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard | MenuMode.TransportRecording | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_GPSPermissionButton, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard | MenuMode.TransportRecording | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_BluetoothEnabledButton, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard | MenuMode.TransportRecording | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_BluetoothPermissionsButton, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard | MenuMode.TransportRecording | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("CO2MonitorPickerStackLayout"), MenuMode.Standard | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_StatusLabel, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard | MenuMode.TransportRecording | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_DeviceLabel, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard | MenuMode.TransportRecording | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_LocationLabel, MenuMode.Standard | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("SearchRangeStackLayout"), MenuMode.Standard | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_UpdateLocationsButton, MenuMode.Standard | MenuMode.TransportSelection | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(this.FindByName<VerticalStackLayout>("LocationStackLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<VerticalStackLayout>("TransitOriginStackLayout"), MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(this.FindByName<VerticalStackLayout>("TransitDestinationStackLayout"), MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(this.FindByName<VerticalStackLayout>("TransitLineStackLayout"), MenuMode.TransportSelection | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(_ResumeRecordingButton, 0);
            MenuModesOfUIElements.Add(_StartRecordingButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(_StartManualRecordingButton, 0);
            //MenuModesOfUIElements.Add(_FinishRecordingButton, MenuMode.Recording | MenuMode.ManualRecording); //stack is hidden so shouldnt be necessary?
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("PrerecordingLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(_OpenMapButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(_OpenImprintButton, MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("DeleteLastSubmissionButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(_LocationLabelRecording, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(_ConfirmCancelRecordingButton, 0);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("RecordingModeButtonStackLayout"), MenuMode.Recording | MenuMode.ManualRecording | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackManualName"), MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackManualAddress"), MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(_LineChartView, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(this.FindByName<Label>("RecordedDataLabel"), MenuMode.Recording | MenuMode.ManualRecording);
            //MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("TrimSliderLayout"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Label>("TrimSliderInfoText"), MenuMode.Recording | MenuMode.ManualRecording | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackNotes"), MenuMode.Recording | MenuMode.ManualRecording | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("StackCheckboxesDoor"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("StackCheckboxesVentilation"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<VerticalStackLayout>("StackDeviceNameFilter"), MenuMode.Standard | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_TrimSlider, MenuMode.Recording | MenuMode.ManualRecording | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(_CO2DeviceNameFilterEditor, MenuMode.Standard | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_VersionLabel, MenuMode.Standard | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_DebugLogButton, MenuMode.Standard | MenuMode.Recording | MenuMode.ManualRecording | MenuMode.TransportRecording | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_StartTransportRecordingButton, MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_BuildingModeButton, MenuMode.Standard | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_TransitModeButton, MenuMode.Standard | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("TransitFilterGrid"), MenuMode.TransportSelection | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackLocationTextFilter"), MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_mapViewExpander, MenuMode.Standard);
            MenuModesOfUIElements.Add(_LocationInfoLabel, MenuMode.Standard | MenuMode.TransportSelection | MenuMode.TransportRecording);
            MenuModesOfUIElements.Add(_ButtonBuildingTransitSelectionStackLayout, MenuMode.Standard | MenuMode.TransportSelection);
            MenuModesOfUIElements.Add(_GetCachedLocationsButton, MenuMode.Standard | MenuMode.TransportSelection | MenuMode.TransportRecording);

        }

        private async void InitUILayout()
        {
            var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
            // Calculate 70% of the screen width
            var buttonWidth70Percent = screenWidth * 0.70;
            var buttonWidth60Percent = screenWidth * 0.60;
            var buttonWidth50Percent = screenWidth * 0.50;
            var buttonWidth40Percent = screenWidth * 0.40;
            var buttonWidth30Percent = screenWidth * 0.30;
            var buttonWidth25Percent = screenWidth * 0.25;

            _LocationUpdateGrid.MinimumWidthRequest = buttonWidth70Percent;
            // Set the button's minimum width
            _ResumeRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
            _StartRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
            _StartManualRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
            _StartTransportRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
            //_UpdateLocationsButton.MinimumWidthRequest = buttonWidth70Percent;
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
            _TransitModeButton.MinimumWidthRequest = buttonWidth40Percent;
            _BuildingModeButton.MinimumWidthRequest = buttonWidth40Percent;
            _LocationUpdateGrid.WidthRequest = _StartRecordingButton.Width;
            _GetCachedLocationsButton.HeightRequest = _StartRecordingButton.Height;
            CollapseExpanderWithDelay(500);
        }

        private async void CollapseExpanderWithDelay(int delayMilliseconds)
        {
            // Wait for the specified delay
            await Task.Delay(delayMilliseconds);

            // Collapse the expander after the delay
            _mapViewExpander.IsExpanded = false;
        }


        public async void UpdateUI()
        {
            loc = LocalisationResourceManager.Instance;
            Application.Current.UserAppTheme = Application.Current.RequestedTheme;
            _VersionLabel.Text = appVersion;
            UpdateStaticMenuItems();
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
            UpdateStartTransitRecordingButton();
            UpdateFinishRecordingButton();
            UpdateFavouredBuildingIcon();
            UpdateFavouredTransitLineIcon();
            UpdateFavouredTransitOriginIcon();
            UpdateFavouredTransitDestinationIcon();

            UpdateLocationMap();
            UpdateMapViewExpander();
            UpdateBackgroundColor();
            UpdateQuickGuideButton();
            //HideElementsWithStatusOK();
        }

        private void UpdateStaticMenuItems()
        {
            //this is ugly but release builds on iPhone dont work with xaml bindings for some reason...

            _QuickGuideButtonTop.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuideUserManualButton));
            _QuickGuideButtonBottom.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuideUserManualButton));
            _DevicePickerLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.MonitorSelectionLabel));
            _BuildingModeButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.TypeSelectionButton_Buildings));
            _TransitModeButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.TypeSelectionButton_Transit));
            _SearchRange.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.SearchRangeLabel));
            _UpdateLocationsButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.UpdateLocationsButton));
            _StartRecordingButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.StartRecordingButton));
            _StartTransportRecordingButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.StartTransitRecordingButton));
            _OpenMapButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.OpenMapInBrowserButton));            
            _OpenImprintButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DataProtectionAndImprintButton));
            _DeleteLastSubmissionButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeletePreviousSubmissionButton));
            //TODO: FilterEditor Label
            _DebugLogButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.CopyDebugLogToClipBoardButton));
            _CrashLogButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.CopyCrashLogToClipBoardButton));
            _TransitFilterAllButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.TypeOfTransitAll));
            _TransitFilterBusButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.TypeOfTransitBus));
            _TransitFilterTramButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.TypeOfTransitTram));
            _TransitFilterSubwayButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.TypeOfTransitSubway));
            _TransitFilterLightRailButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.TypeOfTransitLightrail));
            _TransitFilterTrainButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.TypeOfTransitTrain));
            _PrerecordingLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.Include15mBeforeRecordingToggle));
            _RequestCancelRecordingButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.CancelRecordingButton));
            _TrimSliderInfoText.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.SliderInfoLabel));
            _VentilationLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.VentilationSystemCheckBox));
            _OpenWindowsLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.OpenDoorsWindowsCheckbox));
            _FilterDeviceByNameLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.FilterDeviceByNameLabel));
            

            

            

            //TODO: add all the other Elements
        }

        private async void HideElementsWithStatusOK()
        {
            bool hiddenElementThisTime = false;
            if (gpsActive && gpsGranted == btGranted == btActive)
            {
                if (_StatusButtons.IsVisible == true || _StatusLabel.IsVisible == true) hiddenElementThisTime = true;
                _StatusButtons.IsVisible = false;
                _StatusLabel.IsVisible = false;
            }
            else
            {
                _StatusButtons.IsVisible = true;
                _StatusLabel.IsVisible = true;
            }

            if (SpatialManager.currentLocation.Latitude != 0 || SpatialManager.currentLocation.Longitude != 0)
            {
                if (_LocationLabel.IsVisible == true) hiddenElementThisTime = true;
                _LocationLabel.IsVisible = false;
            }
            else _LocationLabel.IsVisible = true;

            if (_LocationInfoLabel.Text.Length < 1)
            {
                if (_LocationInfoLabel.IsVisible == true) hiddenElementThisTime = true;
                _LocationInfoLabel.IsVisible = false;
            }
            else if (currentMenuMode == MenuMode.Standard)
            {
                _LocationInfoLabel.IsVisible = true;
            }

            if (hiddenElementThisTime)
            {
                await _MainScrollView.ScrollToAsync(0, 0, true);
            }

        }

        private void UpdateBackgroundColor()
        {


            BoxView topStroke = this.FindByName<BoxView>("TopStroke");
            if (topStroke != null)
            {
                var co2 = BluetoothManager.currentCO2Reading;
                if (co2 == 0)
                {
                    topStroke.Color = Color.FromArgb("#00008b");
                }
                else if (co2 < 600)
                {
                    topStroke.Color = Color.FromArgb("#00008b");
                }
                else if (co2 < 800)
                {
                    topStroke.Color = Color.FromArgb("#add8e6");
                }
                else if (co2 < 1000)
                {
                    topStroke.Color = Color.FromArgb("#ffff00");
                }
                else if (co2 < 1200)
                {
                    topStroke.Color = Color.FromArgb("#ffa500");
                }
                else
                {
                    topStroke.Color = Color.FromArgb("#ff0000");
                }
            }
        }

        private void UpdateLocationMap()
        {
            var pickedLocation = (LocationData)_LocationPicker.SelectedItem;
            if (!_mapViewExpander.IsExpanded) return; //we only update if map is actually visible
            if (pickedLocation == null)
            {
                UpdateMap(43.7628933, 11.2547348); //Default set to Florence 
            }
            else if (previousMapLocation != null && previousMapLocation != pickedLocation)
            {
                UpdateMap(pickedLocation.Latitude, pickedLocation.Longitude);
                previousMapLocation = pickedLocation;
            }
            else if (previousMapLocation == null)
            {
                UpdateMap(pickedLocation.Latitude, pickedLocation.Longitude);
                previousMapLocation = pickedLocation;
            }

        }

        private void UpdateMapViewExpander()
        {
            if (!_mapViewExpander.IsExpanded)
            {
                _mapViewExpanderLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.ShowOnMapFoldOut));
            }
            else
            {
                _mapViewExpanderLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.HideMapLabel));
            }
        }

        private void UpdateGetLocationButton()
        {
            if (SpatialManager.currentLocation.Latitude == 0 && SpatialManager.currentLocation.Longitude == 0)
            {
                _UpdateLocationsButton.IsEnabled = false;
            }
            _UpdateLocationsButton.IsEnabled = true;
        }

        private void UpdateStartRecordingButton()
        {
            if (gpsActive && gpsGranted && btGranted && btActive && OverpassModule.BuildingLocationData.Count > 0 && BluetoothManager.discoveredDevices != null && BluetoothManager.discoveredDevices.Count > 0 && BluetoothManager.currentCO2Reading > 0)
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

        private void UpdateStartTransitRecordingButton()
        {
            if (gpsActive && gpsGranted && btGranted && btActive && OverpassModule.TransportStartLocationData.Count > 0 && OverpassModule.TransitLines.Count > 0 && BluetoothManager.discoveredDevices != null && BluetoothManager.discoveredDevices.Count > 0 && BluetoothManager.currentCO2Reading > 0)
            {
                _StartTransportRecordingButton.IsEnabled = true;
            }
            else
            {
                _StartTransportRecordingButton.IsEnabled = false;
            }
        }

        private void UpdateFinishRecordingButton()
        {
            _FinishRecordingButton.IsEnabled = false;
            int original = BluetoothManager.recordedData.Count;
            int trimStart = (int)Math.Floor(_TrimSlider.RangeStart);
            int trimEnd = (int)Math.Floor(_TrimSlider.RangeEnd);
            if (trimEnd - trimStart >= 4 && BluetoothManager.isRecording)
            {

                if (submissionMode == SubmissionMode.Transit)
                {
                    selectedTransitLine = (TransitLineData)_TransitLinePicker.SelectedItem;
                    if (selectedTransitLine == null)
                    {
                        _FinishRecordingButton.IsEnabled = false;
                        _FinishRecordingButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.SubmitRecordingButtonTransitLineMissing));
                    }
                    else
                    {
                        _FinishRecordingButton.IsEnabled = true;
                        _FinishRecordingButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.SubmitRecordingButtonOkay));
                    }
                }

                else if (submissionMode == SubmissionMode.BuildingManual && (_ManualNameEditor.Text == null || _ManualAddressEditor.Text == null))
                {
                    _FinishRecordingButton.IsEnabled = false;
                    //_FinishRecordingButton.Text = "Submit data (needs address & name)";
                }
                else if (submissionMode == SubmissionMode.BuildingManual && (_ManualNameEditor.Text.Length < 1 || _ManualAddressEditor.Text.Length < 1))
                {
                    _FinishRecordingButton.IsEnabled = false;
                    //_FinishRecordingButton.Text = "Submit data (needs address & name)";
                }
                else 
                {
                    _FinishRecordingButton.IsEnabled = true;
                    _FinishRecordingButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.SubmitRecordingButtonOkay));
                }
            }
            else if (BluetoothManager.isRecording)
            {
                _FinishRecordingButton.IsEnabled = false;
                _FinishRecordingButton.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.SubmitRecordingButtonNotEnoughData));
            }


        }

        private void UpdateDeviceLabel()
        {
            try
            {
                if (!btGranted || !btActive)
                {
                    _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.BluetoothInactiveOrPermissionsMissing)); 
                }
                else if (BluetoothManager.discoveredDevices == null || BluetoothManager.discoveredDevices.Count == 0)
                {
                    if (_CO2DeviceNameFilterEditor.Text != null && _CO2DeviceNameFilterEditor.Text.Length > 0 && currentMenuMode != MenuMode.TransportRecording && currentMenuMode != MenuMode.Recording)
                    {
                        _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceNotYetFound)) + " " + LocalisationResourceManager.Instance.GetString(nameof(AppStrings.NameFilterInfo)) + " " + $"{_CO2DeviceNameFilterEditor.Text}";
                    }
                    else if (_CO2DeviceNameFilterEditor.Text != null && _CO2DeviceNameFilterEditor.Text.Length > 0 && (currentMenuMode == MenuMode.TransportRecording || currentMenuMode == MenuMode.Recording))
                    {
                        _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.RecoveringInfo));
                    }
                    else if ((currentMenuMode == MenuMode.TransportRecording || currentMenuMode == MenuMode.Recording))
                    {
                        _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.RecoveringInfo));
                    }
                    else
                    {
                        _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceNotYetFound));
                    }


                    if (BluetoothManager.lastAttemptFailed)
                    {
                        _DeviceLabel.Text += LocalisationResourceManager.Instance.GetString(nameof(AppStrings.PreviousUpdateFailed));
                    }
                }
                //else if (BluetoothManager.sensorUpdateInterval > 60)
                //{
                //    _DeviceLabel.Text = "Device found but Update Interval not set to 1 Minute, change to 1 Minute using official App. next attempt in " + BluetoothManager.timeToNextUpdate + "s";
                //}
                else if (BluetoothManager.currentCO2Reading != 0 && BluetoothManager.gattStatus == 0) //TODO also add check if last reading was a success maybe?         
                {
                    //checking bondstate does not work for Aranet4 or BLE devices in general, would need to infer from trying to read characteristics
                    //if (BluetoothManager.isBonded == false && (monitorType == CO2MonitorType.Airvalent || monitorType == CO2MonitorType.Aranet4))
                    //{
                    //    _DeviceLabel.Text = "Sensor found, but not paired with your Phone, restart the App and enter the PIN displayed on the Sensor when requested)";
                    //}

                    if (monitorType == CO2MonitorType.Aranet4 || monitorType == CO2MonitorType.Airvalent)
                    {
                        //_DeviceLabel.Text = "CO2 Levels: " + BluetoothManager.currentCO2Reading + " |  Update in: " + BluetoothManager.timeToNextUpdate + "s" + "\r\n | rssi: " + BluetoothManager.rssi + " | id: " + BluetoothManager.deviceName;
                        _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceInfoLabel_CO2levels)) + " " + BluetoothManager.currentCO2Reading + " | " + LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceInfoLabel_UpdateIn)) + BluetoothManager.timeToNextUpdate + "s" + "\r\n  | id: " + BluetoothManager.deviceName;
                        if(BluetoothManager.lowCO2ValueDetected==true)
                        {
                            _DeviceLabel.FormattedText = new FormattedString
                            {
                                Spans =
                                {
                                    new Span { Text =  LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceInfoLabel_CO2levels)) + " " + BluetoothManager.currentCO2Reading + " " },
                                    new Span
                                    {
                                        Text =  LocalisationResourceManager.Instance.GetString(nameof(AppStrings.LowCO2Levels)),
                                        TextColor = Colors.OrangeRed, // Pick your desired warning color
                                        FontAttributes = FontAttributes.Bold
                                    },
                                    new Span { Text = "| " + LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceInfoLabel_UpdateIn)) + BluetoothManager.timeToNextUpdate + "s" },
                                    new Span { Text = "\r\n | id: " + BluetoothManager.deviceName }
                                }
                            };
                        }
                    }
                    else
                    {
                        var secondsSinceLastUpdate = DateTime.Now - BluetoothManager.timeOfLastNotifyUpdate;
                        _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceInfoLabel_CO2levels)) + BluetoothManager.currentCO2Reading + " |  Updated " + secondsSinceLastUpdate.Seconds + " seconds ago" + "\r\n | rssi: " + BluetoothManager.rssi + " | id: " + BluetoothManager.deviceName;
                        //_DeviceLabel.Text = "CO2 Levels: " + BluetoothManager.currentCO2Reading + " |  Updated " + secondsSinceLastUpdate.Seconds + " seconds ago" + "\r\n | rssi: " + BluetoothManager.rssi + " | id: " + BluetoothManager.deviceName;
                        if (BluetoothManager.lowCO2ValueDetected == true)
                        {
                            _DeviceLabel.FormattedText = new FormattedString
                            {
                                Spans =
                                {
                                    new Span { Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceInfoLabel_CO2levels)) + " " + BluetoothManager.currentCO2Reading + " " },
                                    new Span
                                    {
                                        Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.LowCO2Levels)),
                                        TextColor = Colors.OrangeRed, // Pick your desired warning color
                                        FontAttributes = FontAttributes.Bold
                                    },
                                    new Span { Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceInfoLabel_UpdateIn)) + " " + BluetoothManager.timeToNextUpdate + "s" },
                                    new Span { Text = "\r\n | id: " + BluetoothManager.deviceName }
                                }
                            };
                        }
                    }

                    if (BluetoothManager.lastAttemptFailed)
                    {
                        _DeviceLabel.Text += LocalisationResourceManager.Instance.GetString(nameof(AppStrings.PreviousUpdateFailed));
                    }
                }
                else if(BluetoothManager.currentCO2Reading != 0 && monitorType == CO2MonitorType.AirSpot)
                {
                    _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceInfoLabel_CO2levels)) + " " + BluetoothManager.currentCO2Reading + " | "+ LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DeviceInfoLabel_UpdateIn)) + BluetoothManager.timeToNextUpdate + "s" + "\r\n | rssi: " + BluetoothManager.rssi + " | id: " + BluetoothManager.deviceName;
                }


                else if (BluetoothManager.currentCO2Reading == 0 && BluetoothManager.isGattA2DP == true)
                {
                    _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DisabledSmarthomeIntegration));
                }
                else if (BluetoothManager.outdatedVersion == true)
                {
                    _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.OutDatedFirmware));
                }
                else if (BluetoothManager.currentCO2Reading == 0)
                {
                    if (monitorType == CO2MonitorType.Aranet4 || monitorType == CO2MonitorType.Airvalent)
                    {
                        _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.FirstUpdate)) + BluetoothManager.timeToNextUpdate + "s" + "\r\n | rssi: " + BluetoothManager.rssi + " | Gatt Status: " + BluetoothManager.gattStatus;
                    }
                    else
                    {
                        _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.WaitingForFirstUpdate)) + "\r\n | rssi: " + BluetoothManager.rssi + " | Gatt status: " + BluetoothManager.gattStatus;
                    }

                    if (BluetoothManager.lastAttemptFailed)
                    {
                        _DeviceLabel.Text += LocalisationResourceManager.Instance.GetString(nameof(AppStrings.PreviousUpdateFailed));
                    }
                }
            }
            catch
            {
                _DeviceLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.UpdateFailedNextIn)) + " " + BluetoothManager.timeToNextUpdate;
                //Debug.WriteLine("UpdateDeviceLabel - exception caught");
            }


        }

        private void UpdateStatusLabel()
        {
            {
                _StatusLabel.Text = ("");
                _StatusLabel.IsVisible = true;
                if (!gpsActive) _StatusLabel.Text += LocalisationResourceManager.Instance.GetString(nameof(AppStrings.GPSNotEnabled));
                if (!gpsGranted) _StatusLabel.Text += LocalisationResourceManager.Instance.GetString(nameof(AppStrings.LocationPermissionMissing));
                if (!btActive) _StatusLabel.Text +=  LocalisationResourceManager.Instance.GetString(nameof(AppStrings.BluetoothNotEnabled));
                if (!btGranted) _StatusLabel.Text += LocalisationResourceManager.Instance.GetString(nameof(AppStrings.BluetoothPermissionMissing));
                if (gpsActive && gpsGranted && btActive && btGranted)
                {
                    _StatusLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.GPSAndBTOkay));
                    _StatusLabel.IsVisible = false;
                }
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

            btActive = bluetoothHelper.CheckIfBTEnabled();
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

            btGranted = bluetoothHelper.CheckStatus();
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
                    _LocationLabel.IsVisible = false;
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
                    _LocationLabel.IsVisible = true;
                    _LocationLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.DuringFirstGPSLocationUpdate));
                }
            }
            else
            {
                _LocationLabel.IsVisible = true;
                _LocationLabel.Text = ("");
                if (!gpsActive) _LocationLabel.Text += LocalisationResourceManager.Instance.GetString(nameof(AppStrings.GPSNotEnabled));
                if (!gpsGranted) _LocationLabel.Text += LocalisationResourceManager.Instance.GetString(nameof(AppStrings.LocationPermissionMissing));
            }
        }

        private void UpdateFavouredBuildingIcon()
        {

            if (_LocationPicker.SelectedItem == null)
            {
                _StarIconToggleBuilding.Source = "star_icon.png";
                return;
            }
            LocationData d = (LocationData)_LocationPicker.SelectedItem;
            if (favouredLocations.Contains(d.Type + "_" + d.ID.ToString()))
            {
                _StarIconToggleBuilding.Source = "star_icon_active.png";
            }
            else
            {
                _StarIconToggleBuilding.Source = "star_icon.png";
            }
        }

        private void UpdateQuickGuideButton()
        {
            if (evertransmittedSucessfully)
            {
                _QuickGuideButtonBottom.IsVisible = true;
                _QuickGuideButtonTop.IsVisible = false;
            }
            else
            {
                _QuickGuideButtonBottom.IsVisible = false;
                _QuickGuideButtonTop.IsVisible = true;
            }
        }

        private void UpdateFavouredTransitOriginIcon()
        {
            if (_TransitOriginPicker.SelectedItem == null)
            {
                _StarIconToggleTransitOrigin.Source = "star_icon.png";
                return;
            }
            LocationData d = (LocationData)_TransitOriginPicker.SelectedItem;
            if (favouredLocations.Contains(d.Type + "_" + d.ID.ToString()))
            {
                _StarIconToggleTransitOrigin.Source = "star_icon_active.png";
            }
            else
            {
                _StarIconToggleTransitOrigin.Source = "star_icon.png";
            }
        }

        private void UpdateFavouredTransitDestinationIcon()
        {
            if (_TransitDestinationPicker.SelectedItem == null)
            {
                _StarIconToggleTransitDestination.Source = "star_icon.png";
                return;
            }
            LocationData d = (LocationData)_TransitDestinationPicker.SelectedItem;
            if (favouredLocations.Contains(d.Type + "_" + d.ID.ToString()))
            {
                _StarIconToggleTransitDestination.Source = "star_icon_active.png";
            }
            else
            {
                _StarIconToggleTransitDestination.Source = "star_icon.png";
            }
        }


        private void UpdateFavouredTransitLineIcon()
        {

            if (_TransitLinePicker.SelectedItem == null)
            {
                _StarIconToggleTransitLine.Source = "star_icon.png";
                return;
            }
            TransitLineData d = (TransitLineData)_TransitLinePicker.SelectedItem;
            if (favouredLocations.Contains(d.NWRType + "_" + d.ID.ToString()))
            {
                _StarIconToggleTransitLine.Source = "star_icon_active.png";
            }
            else
            {
                _StarIconToggleTransitLine.Source = "star_icon.png";
            }
        }

        private void UpdateLocationRecordingLabel()
        {
            if (currentMenuMode == MenuMode.Recording && selectedLocation != null)
            {                
                {                    
                    _LocationLabelRecording.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.RecordedDataLabel)) + " " + selectedLocation.Name;
                }             
                
            }
            else if(currentMenuMode == MenuMode.Recording && selectedLocation == null)
            {
                _LocationLabelRecording.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.NoSelectedLocation)); //shouldnt even happen
            }
            else if(currentMenuMode == MenuMode.TransportRecording && selectedTransitOriginLocation != null)
            {
                _LocationLabelRecording.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.TransitOrigin)) + selectedTransitOriginLocation;
            }
            else if(currentMenuMode == MenuMode.TransportRecording && selectedTransitOriginLocation == null)
            {
                _LocationLabelRecording.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.NoTransitOriginSelected)); //shouldnt even happen
            }
        }

        private void UpdateLocationInfoLabel()
        {

            if (OverpassModule.everFetchedLocations == false && currentMenuMode == MenuMode.Standard)
            {                                
                _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.UpdateLocationsButtonDefaultInfoLabel)); 
            }            
            else if (OverpassModule.lastFetchWasSuccessButNoResults)
            {
                _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.NoLocationsInRange));
            }
            else if (OverpassModule.lastFetchWasSuccess && (currentMenuMode == MenuMode.Standard))
            {
                _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.UpdateLocationsButtonFoundLocationsLabel));
            }
            else if (OverpassModule.lastFetchWasSuccess && (currentMenuMode == MenuMode.TransportRecording || currentMenuMode == MenuMode.TransportSelection))
            {
                _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.LocationsFoundStatus));
            }
            else if (OverpassModule.lastFetchWasSuccess == false && !OverpassModule.currentlyFetching)
            {
                if(OverpassModule.lastFetchWasATimeout)
                {
                    _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.LocationSearchTimeout));                  
                }
                else
                {
                    if(OverpassModule.everFetchedTransitLocations && currentMenuMode != MenuMode.Standard)
                    {
                        _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.UpdateLocationsFailed));
                    }
                    else if(OverpassModule.everFetchedLocations && currentMenuMode == MenuMode.Standard)
                    {
                        _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.UpdateLocationsFailed));
                    }
                    
                }                
            }
            else if (OverpassModule.currentlyFetching)
            {
                _LocationInfoLabel.Text = $"{LocalisationResourceManager.Instance.GetString(nameof(AppStrings.UpdateLocationsButtonSearchingInfoLabel))} ({Math.Round((DateTime.Now-OverpassModule.startTimeOfFetch).TotalSeconds)}s) ";
                if(OverpassModule.isAlreadyRetry && OverpassModule.useAlternative)
                {
                    _LocationInfoLabel.Text = $"{LocalisationResourceManager.Instance.GetString(nameof(AppStrings.AlternateLocationInfo))} ({Math.Round((DateTime.Now - OverpassModule.startTimeOfFetch).TotalSeconds)}s)";
                }
            }

            if (OverpassModule.everFetchedTransitLocations == false && currentMenuMode == MenuMode.TransportSelection)
            {
                _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.UpdateLocationUsageInfo));
            }

            if (OverpassModule.lastFetchWasFromCachedData)
            {
                if(currentMenuMode == MenuMode.Standard && OverpassModule.BuildingLocationData != null && OverpassModule.BuildingLocationData.Count>0)
                {
                    _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.RetrievedStored));
                }
                else if(currentMenuMode == MenuMode.Standard)
                {
                    _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.NoStoredLocations));
                }
                else if(currentMenuMode == MenuMode.TransportSelection && OverpassModule.TransportStartLocationData != null && OverpassModule.TransportStartLocationData.Count>0)
                {
                    _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.RetrievedStored));
                }
                else if (currentMenuMode == MenuMode.TransportSelection)
                {
                    _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.NoStoredLocations));
                }
                else if (currentMenuMode == MenuMode.TransportRecording && OverpassModule.TransportDestinationLocationData != null && OverpassModule.TransportDestinationLocationData.Count > 0)
                {
                    _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.RetrievedStored));
                }
                else if (currentMenuMode == MenuMode.TransportRecording)
                {
                    _LocationInfoLabel.Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.NoStoredLocations));
                }
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

            if (maxSliderVal > previousDataCount) //maybe must be > instead of >=
            {
                int max = Math.Max(1, maxSliderVal);                
                if (_TrimSlider.RangeEnd == max - 1 || previousDataCount==_TrimSlider.RangeEnd || endtrimSliderIsAtmax) _TrimSlider.RangeEnd = max;
                previousDataCount = maxSliderVal;
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
            currentMenuMode = mode;
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

        public async void ChangeToStandardUI(bool manualTriggered)
        {
            ChangeToUI(MenuMode.Standard);
            _TransitModeButton.BackgroundColor = Colors.LightGray;
            _TransitModeButton.TextColor = Colors.Black;
            _BuildingModeButton.BackgroundColor = Color.Parse("#512BD4");
            _BuildingModeButton.TextColor = Colors.White;

            if (RecoveryData.recordingMode == "Building" || RecoveryData.recordingMode == "Transit")
            {
                _ResumeRecordingButton.IsVisible = true;
            }
            else
            {
                _ResumeRecordingButton.IsVisible = false;
            }
            if(!manualTriggered)
            {
                await _MainScrollView.ScrollToAsync(0, 0, true);
            }
            //HideElementsWithStatusOK();
        }    

        public async void ChangeToRecordingUI()
        {
            ChangeToUI(MenuMode.Recording);
            await _MainScrollView.ScrollToAsync(0, 0, true);
            //HideElementsWithStatusOK();
        }

        public async void ChangeToManualRecordingUI()
        {
            ChangeToUI(MenuMode.ManualRecording);
            await _MainScrollView.ScrollToAsync(0, 0, true);
            //HideElementsWithStatusOK();
        }

        public async void ChangeToTransportRecordingUI()
        {
            ChangeToUI(MenuMode.TransportRecording);
            await _MainScrollView.ScrollToAsync(0, 0, true);
            //HideElementsWithStatusOK();
        }

        public async void ChangeToTransportSelectionUI(bool manualTriggered)
        {
            if (RecoveryData.recordingMode == "Building" || RecoveryData.recordingMode == "Transit")
            {
                _ResumeRecordingButton.IsVisible = true;
            }
            else
            {
                _ResumeRecordingButton.IsVisible = false;
            }
            ChangeToUI(MenuMode.TransportSelection);
            if(!manualTriggered)
            {
                await _MainScrollView.ScrollToAsync(0, 0, true);
            }
            //HideElementsWithStatusOK();
        }
    }

}
