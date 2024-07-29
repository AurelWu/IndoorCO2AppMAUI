
//TODO: check if works on Android
//TODO: Overpass Module
//TODO: iphone Developer Account ?!?
//TODO: Layout
//TODO: Button Functionality

using IndoorCO2App.Controls;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;

namespace IndoorCO2App;


//TODO: create lineChart
//TODO: create double Sided Slider
//TODO: create Loggint TextView
//TODO: fix that first minute the value of t-1 is also shown, which then later disappears (cause probably that minimum 2 values are fetched?)11

public partial class MainPage : ContentPage
{
    private const string SelectedMonitorPreferenceKey = "SelectedMonitorIndex";
    bool firstInit = true;
    public bool manualRecordingMode = false;
    bool hideLocation = true;

    bool startTrimSliderHasBeenUsed = false;
    bool endTrimSliderHasBeenUsed = false;
    bool endtrimSliderIsAtmax = true;

    public static int startTrimSliderValue = 0;
    public static int endTrimSliderValue = 1;
    public static int previousDataCount = 0;

    bool gpsGranted = false;
    bool gpsActive = false;
    bool btActive = false;
    bool btGranted = false;

	int count = 0;
    private readonly PeriodicTimer _timer;
    int searchRange = 100;
    private DateTime timeOfLastGPSUpdate = DateTime.MinValue;
    internal List<LocationData> Locations { get; set; }
    LocationData selectedLocation;
    public static MainPage MainPageSingleton;

    public static bool hasOpenWindowsDoors;
    public static bool hasVentilationSystem;
    internal CO2MonitorType monitorType;
    //public static IWakeLockService wakeLockService;

    //public const string CHANNEL_ID = "com.companyname.indoorco2app.channel";

    public MainPage()
	{
        //monitorType = CO2MonitorType.Aranet; //HARDCODED FOR NOW!
        monitorType = CO2MonitorType.Aranet4; //HARDCODED FOR NOW!
        MainPageSingleton = this;
        Locations = new List<LocationData>();        
        //TODO => Overpass search range based on radio button instead of hardcoded 100
        //TODO: => record data when recording mode
        //TODO: => draw Linechart when recording
        //TODO: => add cancel button when recording (with confirmation)
        //TODO: => add submit Data Button (with info if success or not stuff)
        //TODO: => Data submission

        InitializeComponent();
        CO2MonitorPicker.SelectedIndex = 0;
        LoadMonitorType();
        firstInit = false;

        //TODO: if we have a stored value for the selection then we take that instead

        BluetoothManager.Init();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(0.4));
        UISetup();
        SwitchToStandardUI();
        Update();
    }

    private void UISetup()
    {
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        // Calculate 70% of the screen width
        var buttonWidth70Percent = screenWidth * 0.7;
        var buttonWidth60Percent = screenWidth * 0.6;
        var buttonWidth50Percent = screenWidth * 0.5;
        var buttonWidth30Percent = screenWidth * 0.3;
        var buttonWidth25Percent = screenWidth * 0.25;

        // Set the button's minimum width
        StartRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
        StartManualRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
        UpdateLocationsButton.MinimumWidthRequest = buttonWidth70Percent;
        OpenMapButton.MinimumWidthRequest = buttonWidth70Percent;
        OpenImprintButton.MinimumWidthRequest = buttonWidth70Percent;
        FinishRecordingButton.MinimumWidthRequest = buttonWidth60Percent;
        FinishRecordingButton.MaximumWidthRequest = buttonWidth60Percent;
        RequestCancelRecordingButton.MinimumWidthRequest = buttonWidth25Percent;
        RequestCancelRecordingButton.MaximumWidthRequest = buttonWidth25Percent;
        ConfirmCancelRecordingButton.MinimumWidthRequest = buttonWidth25Percent;
        ConfirmCancelRecordingButton.MaximumWidthRequest = buttonWidth25Percent;        
        RadioButton100m.IsChecked = true;
    }

    private void SwitchToRecordingUI()
    {
        NotesEditor.Text = "";
        //wakeLockService.AcquireWakeLock();
        //StackRangeTrimmer.IsVisible = true;        
        FinishRecordingButton.Text = "Submit Data";
        FinishRecordingButton.IsEnabled = true;
        FinishRecordingButton.IsVisible = true;
        RequestCancelRecordingButton.IsVisible = true;
        ConfirmCancelRecordingButton.IsVisible = false;
        //RecordedDataLabel.IsVisible = true; //RecordedDataLabel is temporary instead of lineChart
        LocationLabelRecording.IsVisible = true;
        LocationLabel.IsVisible = false;
        SearchRangeStackLayout.IsVisible = false;
        LocationStackLayout.IsVisible = false;
        StartRecordingButton.IsVisible = false;
        StartManualRecordingButton.IsVisible = false;
        OpenImprintButton.IsVisible = false;
        OpenMapButton.IsVisible = false;
        UpdateLocationsButton.IsVisible = false;
        //StackLayoutTrimSliderStart.IsVisible = true;
        //StackLayoutTrimSliderEnd.IsVisible = true;        
        StackCheckboxesDoorVentilation.IsVisible = true;
        StackNotes.IsVisible = true;
        lineChartView.IsVisible = true;
        startTrimSlider.IsVisible = true;
        endTrimSlider.IsVisible = true;
        TrimSliderInfoText.IsVisible = true;
        CO2MonitorPickerStackLayout.IsVisible = false;

        startTrimSliderHasBeenUsed = false;
        endTrimSliderHasBeenUsed = false;
    }

    private void SwitchToManualRecordingUI()
    {
        NotesEditor.Text = "";
        //wakeLockService.AcquireWakeLock();
        //StackRangeTrimmer.IsVisible = true;        
        FinishRecordingButton.Text = "Submit Data";
        FinishRecordingButton.IsEnabled = true;
        FinishRecordingButton.IsVisible = true;
        RequestCancelRecordingButton.IsVisible = true;
        ConfirmCancelRecordingButton.IsVisible = false;
        //RecordedDataLabel.IsVisible = true; //RecordedDataLabel is temporary instead of lineChart
        LocationLabelRecording.IsVisible = false;
        LocationLabel.IsVisible = true;
        SearchRangeStackLayout.IsVisible = false;
        LocationStackLayout.IsVisible = false;
        StartRecordingButton.IsVisible = false;
        StartManualRecordingButton.IsVisible = false;
        OpenImprintButton.IsVisible = false;
        OpenMapButton.IsVisible = false;
        UpdateLocationsButton.IsVisible = false;
        //StackLayoutTrimSliderStart.IsVisible = true;
        //StackLayoutTrimSliderEnd.IsVisible = true;        
        StackCheckboxesDoorVentilation.IsVisible = true;
        StackNotes.IsVisible = true;
        StackManualName.IsVisible = true;
        StackManualAddress.IsVisible = true;
        ManualNameEditor.Text = "";
        ManualAddressEditor.Text = "";
        lineChartView.IsVisible = true;
        startTrimSlider.IsVisible = true;
        endTrimSlider.IsVisible = true;
        TrimSliderInfoText.IsVisible = true;
        CO2MonitorPickerStackLayout.IsVisible = false;

        startTrimSliderHasBeenUsed = false;
        endTrimSliderHasBeenUsed = false;
    }

    private void SwitchToStandardUI() 
    {
        //wakeLockService.ReleaseWakeLock();
        //StackRangeTrimmer.IsVisible = true;
        FinishRecordingButton.IsVisible = false;
        RequestCancelRecordingButton.IsVisible = false;
        ConfirmCancelRecordingButton.IsVisible = false;
        RecordedDataLabel.IsVisible = false; //RecordedDataLabel is temporary instead of lineChart
        LocationLabelRecording.IsVisible = false;
        LocationLabel.IsVisible = true;
        SearchRangeStackLayout.IsVisible = true;
        LocationStackLayout.IsVisible = true;
        StartRecordingButton.IsVisible = true;
        StartManualRecordingButton.IsVisible = true;
        OpenImprintButton.IsVisible = true;
        OpenMapButton.IsVisible = true;
        UpdateLocationsButton.IsVisible = true;
        //StackLayoutTrimSliderStart.IsVisible = false;
        //StackLayoutTrimSliderEnd.IsVisible = false;        
        StackCheckboxesDoorVentilation.IsVisible = false;
        StackNotes.IsVisible = false; ;
        StackManualName.IsVisible = false;
        StackManualAddress.IsVisible = false;
        lineChartView.IsVisible = false;
        startTrimSlider.IsVisible = false;
        endTrimSlider.IsVisible = false;
        TrimSliderInfoText.IsVisible = false;
        CO2MonitorPickerStackLayout.IsVisible = true;
    }

    private void OnUpdateLocationsClicked(object sender, EventArgs e)
    {
        //SemanticScreenReader.Announce(UpdateLocationsButton.Text);
        OverpassModule.FetchNearbyBuildingsAsync(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange,this); //TODO => 
    }

    private void StartRecording(bool manualMode)
    {
        BluetoothManager.recordedData = new List<SensorData>();
        this.manualRecordingMode = manualMode;
        endTrimSlider.Minimum = 0;
        endTrimSlider.Maximum = 1;
        startTrimSlider.Maximum = 1;
        startTrimSliderHasBeenUsed = false;
        endTrimSliderHasBeenUsed = false;
        previousDataCount = 0;
        //Console.WriteLine(LocationPicker);
        if (LocationPicker != null && Locations.Count > 0 && !manualMode)
        {
            if (LocationPicker.SelectedItem != null)
            {
                selectedLocation = (LocationData)LocationPicker.SelectedItem;
            }
            SwitchToRecordingUI();            
            BluetoothManager.StartNewRecording(selectedLocation, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }
        else if (manualMode)
        {
            SwitchToManualRecordingUI();
            BluetoothManager.StartNewManualRecording(selectedLocation, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }
    }

    private void OnStartRecordingClicked(object sender, EventArgs e)
    {
        StartRecording(false);
    }



    private async void OnStartManualRecordingClicked(object sender, EventArgs e)
    {
        //shows pop up with warning to only use if really needed.
        string msg = "Only use this recording mode if the location is not in the List above or if receiving Locations does not work currently. Recordings in this mode are not put into the map instantly but manually looked at and then added to the Map if the location can be validated. Using this mode the exact GPS Coordinates taken during the recording duration will be submitted at the end!";

        bool result = await DisplayAlert("Manual Recording Mode", msg, "Understood", "Cancel");
        if (result==true)
        {            
            StartRecording(true);
        }
        else
        {
            
        }
    }

    private void OnFinishRecordingClicked(object sender, EventArgs e)
    {
        FinishRecordingButton.Text = "Submitting Data";
        FinishRecordingButton.IsEnabled = false; ;
        int trimStart = (int)Math.Floor(startTrimSlider.Value);
        int trimEnd = (int)Math.Floor(endTrimSlider.Value);
        BluetoothManager.FinishRecording(trimStart,trimEnd,manualRecordingMode, ManualNameEditor.Text, ManualAddressEditor.Text);
    }
    
    private async void OnRequestCancelRecordingClicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Cancel Recording", "Are you sure you want to cancel the recording?", "Yes", "No");
        if (result == true)
        {
            CancelRecording();
        }
        else
        {

        }
    }

    private void OnConfirmCancelRecordingClicked(object sender, EventArgs e)
    {
        CancelRecording();
    }

    private void CancelRecording()
    {
        BluetoothManager.StopRecording();
        SwitchToStandardUI();
    }

    private void OnShowMapInBrowserClicked(object sender, EventArgs e)
    {
        //SemanticScreenReader.Announce(OpenMapButton.Text);
        var url = "https://indoorco2map.com/";
        Launcher.OpenAsync(url);

    }

    private void OnImprintClicked(object sender, EventArgs e)
    {
        var url = "https://www.bluestats.net/Datenschutz.html";
        Launcher.OpenAsync(url);
    }

    private void OnRadioButtonCheckedChanged(object sender, EventArgs e)
    {        
        if (RadioButton50m.IsChecked)
        {
            searchRange = 50;
        }
        else if (RadioButton100m.IsChecked)
        {
            searchRange = 100;
        }
        else if (RadioButton250m.IsChecked)
        {
            searchRange = 250;
        }        
    }


    private async void Update()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync())
            {
                UpdateUI();
                
                
                BluetoothManager.Update(monitorType);

                //non-UI Stuff now done in foreground Service
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

    private void UpdateUI()
    {
        UpdateGPSStatusButton();
        UpdateGPSPermissionButton();
        UpdateBluetoothStatusButton();
        UpdateBluetoothPermissionsButton();
        

        UpdateLocationLabel();
        UpdateLocationRecordingLabel();
        UpdateLocationInfoLabel();
        //UpdateRecordedDataLabel();
        UpdateLineChart();
        UpdateStatusLabel();
        UpdateDeviceLabel();
        UpdateStartRecordingButton();
        UpdateFinishRecordingButton();
    }

    private void UpdateStartRecordingButton()
    {
        if(gpsActive && gpsGranted && btGranted && btActive && OverpassModule.LocationData.Count > 0 && BluetoothManager.discoveredDevices!=null && BluetoothManager.discoveredDevices.Count>0 && BluetoothManager.currentCO2Reading > 0)
        {                        
            StartRecordingButton.IsEnabled = true;         
        }
        else
        {
            StartRecordingButton.IsEnabled = false;
         
        }

        if (gpsActive && gpsGranted && btGranted && btActive && OverpassModule.everFetchedLocations==true && OverpassModule.currentlyFetching==false && BluetoothManager.discoveredDevices != null && BluetoothManager.discoveredDevices.Count > 0 && BluetoothManager.currentCO2Reading > 0)
        {        
            StartManualRecordingButton.IsEnabled = true;
        }
        else
        {
            StartManualRecordingButton.IsEnabled = false;
        }
    }

    private void UpdateFinishRecordingButton()
    {
        int original = BluetoothManager.recordedData.Count;
        int trimStart = (int)Math.Floor(startTrimSlider.Value);
        int trimEnd = (int)Math.Floor(endTrimSlider.Value);
        if ( trimEnd-trimStart  >= 5 && BluetoothManager.isRecording)
        {            
            if (manualRecordingMode && (ManualNameEditor.Text.Length < 1 || ManualAddressEditor.Text.Length < 1))
            {
                FinishRecordingButton.IsEnabled = false;
                FinishRecordingButton.Text = "Submit Data (needs Address & Name)";
            }
            else
            {
                FinishRecordingButton.IsEnabled = true;
                FinishRecordingButton.Text = "Submit Data";
            }
        }
        else if(BluetoothManager.isRecording)
        {
            FinishRecordingButton.IsEnabled = false;
            FinishRecordingButton.Text = "Submit Data (needs 5 Minutes of Data)";
        }
        
        
    }

    private void UpdateStatusLabel()
    {
        {
            StatusLabel.Text=("");
            if (!gpsActive) StatusLabel.Text += "GPS not enabled | ";            
            if (!gpsGranted) StatusLabel.Text += "Location Permission missing |";
            if (!btActive) StatusLabel.Text += "Bluetooth not enabled |";
            if (!btGranted) StatusLabel.Text += "Bluetooth permission not granted";
            if (gpsActive && gpsGranted && btActive && btGranted) StatusLabel.Text = "GPS & Bluetoooth Permissions and Status okay";
        }
    }

    private async void UpdateGPSPermissionButton()
    {
        gpsGranted = await SpatialManager.IsLocationPermissionGrantedAsync();
        if(gpsGranted)
        {             
            ButtonGPSPermission.BackgroundColor = Color.Parse("Green");
        }
        else
        {
            ButtonGPSPermission.BackgroundColor = Color.Parse("Red");
        }        
    }

    private void UpdateGPSStatusButton()
    {
        gpsActive = SpatialManager.CheckIfGpsIsEnabled();
        if(gpsActive)
        {
            ButtonGPSStatus.BackgroundColor = Color.Parse("Green");
        }
        else
        {
            ButtonGPSStatus.BackgroundColor = Color.Parse("Red");
        }
    }

    private void UpdateBluetoothStatusButton()
    {
        btActive = BluetoothManager.bluetoothService.IsBluetoothEnabled();
        if(btActive)
        {
            ButtonBluetoothStatus.BackgroundColor = Color.Parse("Green");
        }
        else
        {
            ButtonBluetoothStatus.BackgroundColor = Color.Parse("Red");
        }
    }

    private void UpdateBluetoothPermissionsButton()
    {
        btGranted = BluetoothPermissions.CheckStatus();
        if(btGranted) 
        {
            ButtonBluetoothPermissions.BackgroundColor = Color.Parse("Green");
        }
        else
        {
            ButtonBluetoothPermissions.BackgroundColor = Color.Parse("Red");
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
                    LocationLabel.Text = "Lat: " + SpatialManager.currentLocation.Latitude.ToString("0.######") + " | Lon:" + SpatialManager.currentLocation.Longitude.ToString("0.######")+ " (tap to hide)";
                }
                else
                {
                    LocationLabel.Text = "Lat: " + SpatialManager.currentLocation.Latitude.ToString("0.#") +"****" + " | Lon:" + SpatialManager.currentLocation.Longitude.ToString("0.#"+"****") + " (tap to show)";
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

    private void UpdateLocationInfoLabel()
    {
        if(OverpassModule.everFetchedLocations == false)
        {
            LocationInfoLabel.Text = "Press Update Locations to get nearby locations";
        }
        else if(OverpassModule.lastFetchWasSuccessButNoResults)
        {
            LocationInfoLabel.Text = "No locations in range";
        }
        else if(OverpassModule.lastFetchWasSuccess)
        {
            LocationInfoLabel.Text = "Select Location:";
        }
        else if(OverpassModule.lastFetchWasSuccess == false && !OverpassModule.currentlyFetching)
        {
            LocationInfoLabel.Text = "Update locations request failed, try again later";
        }
        else if(OverpassModule.currentlyFetching)
        {
            LocationInfoLabel.Text = "currently retrieving nearby locations";
        }
    }

    private void UpdateDeviceLabel()
    {
        try
        {
            if (!btGranted || !btActive)
            {
                DeviceLabel.Text = "Bluetooth not enabled or permissions missing, can not fetch Sensor Data";
            }
            else if (BluetoothManager.discoveredDevices.Count == 0)
            {
                DeviceLabel.Text = "Device not yet found. This might take a while.";

                if (BluetoothManager.lastAttemptFailed)
                {
                    DeviceLabel.Text += " | previous update failed";
                }
            }
            else if (BluetoothManager.sensorUpdateInterval > 60)
            {
                DeviceLabel.Text = "Device found but Update Interval not set to 1 Minute, change to 1 Minute using official App. next attempt in " + BluetoothManager.timeToNextUpdate + "s";
            }
            else if (BluetoothManager.currentCO2Reading != 0 && BluetoothManager.gattStatus == 0) //TODO also add check if last reading was a success maybe?         
            {
                DeviceLabel.Text = "CO2 Levels: " + BluetoothManager.currentCO2Reading + " |  initiating Update in: " + BluetoothManager.timeToNextUpdate + "s" + "\r\n | rssi: " + BluetoothManager.rssi + " | Gatt Status: " + BluetoothManager.gattStatus;
                if (BluetoothManager.lastAttemptFailed)
                {
                    DeviceLabel.Text += " | previous update failed";
                }
            }
            else if (BluetoothManager.currentCO2Reading == 0 && BluetoothManager.isGattA2DP == true)
            {
                DeviceLabel.Text = "Sensor found, but the required 'Smart Home Integration' is disabled.\r\n Please enable it using the official Aranet App (use the Gears Icon)";
            }
            else if (BluetoothManager.currentCO2Reading == 0)
            {
                DeviceLabel.Text = "initiating first Update in:" + BluetoothManager.timeToNextUpdate + "s" + "\r\n | rssi: " + BluetoothManager.rssi + " | Gatt Status: " + BluetoothManager.gattStatus;
                if (BluetoothManager.lastAttemptFailed)
                {
                    DeviceLabel.Text += " | previous update failed";
                }
            }
        }
        catch
        {
            DeviceLabel.Text = "update failed - next attempt in: " + BluetoothManager.timeToNextUpdate;
            Debug.WriteLine("UpdateDeviceLabel - exception caught");
        }
        

    }

    private void UpdateLocationRecordingLabel()
    {
        if (selectedLocation != null)
        {
            LocationLabelRecording.Text = "Location: " + selectedLocation.Name;
        }
        else
        {
            LocationLabelRecording.Text = "No Location Selected";
        }
    }

    private void UpdateRecordedDataLabel()
    {
        var formattedString = new FormattedString();

        // Create spans with different colors
        var labelspan = new Span
        {
            Text = "recorded CO2-Values: ",
            //TextColor = Colors.Black
        };

        formattedString.Spans.Add(labelspan);


        try
        {
            for (int i = 0; i < BluetoothManager.recordedData.Count; i++) //what happens if value changes during looping over it? =>
            {
                //var x = testSlider;
                int start = (int)Math.Floor(startTrimSlider.Value);
                int end = (int)(endTrimSlider.Value); //last Index
                if (i < start || i > end)
                {
                    Span s = new Span
                    {
                        Text = BluetoothManager.recordedData[i].CO2ppm.ToString() + " ",
                        TextColor = Colors.Gray
                    };
                    formattedString.Spans.Add(s);
                }
                else
                {
                    Span s = new Span
                    {
                        Text = BluetoothManager.recordedData[i].CO2ppm.ToString() + " ",
                        //TextColor = Colors.Black
                    };
                    formattedString.Spans.Add(s);
                }
                RecordedDataLabel.FormattedText = formattedString;
            }
        }

        catch (Exception e)
        {
            RecordedDataLabel.Text = "retrieving Data failed, next try in 1 Minute"; //maybe keep old Data in case this happens?
        }
    }
    

    private void UpdateLineChart()
    {
        lineChartView.SetData(BluetoothManager.recordedData);
        int maxSliderVal = BluetoothManager.recordedData.Count;

        startTrimSlider.Minimum = 0;
        startTrimSlider.Maximum = maxSliderVal;
        endTrimSlider.Minimum = 0;        
        
        
        if (endTrimSlider.Maximum == 0)
        {
            endTrimSlider.Maximum = 1;
            endTrimSlider.Value = 1;
        }
        else if(maxSliderVal>0) 
        {            
            endTrimSlider.Maximum = maxSliderVal;
            
        }

        if (maxSliderVal > previousDataCount)
        {
            int max = Math.Max(1, maxSliderVal);
            previousDataCount = maxSliderVal;
            if (endTrimSlider.Value == max - 1) endTrimSlider.Value = max;
        }

        if (!endTrimSliderHasBeenUsed && maxSliderVal>0)
        {
            endTrimSlider.Value = endTrimSlider.Maximum;
        }        
        if(endtrimSliderIsAtmax)
        {
            endTrimSlider.Value = endTrimSlider.Maximum;
        }

        if(endTrimSlider.Value == endTrimSlider.Maximum)
        {
            endtrimSliderIsAtmax = true;
        }
        else
        {
            endtrimSliderIsAtmax= false;
        }

        startTrimSliderValue = (int)startTrimSlider.Value;
        endTrimSliderValue = (int)endTrimSlider.Value;        
    }

    private async void OnRequestBluetoothEnableDialog(object sender, EventArgs e)
    {
        bool isActive = BluetoothManager.bluetoothService.IsBluetoothEnabled();
        if (isActive) return; // won't do anything already active
        bool result = await BluetoothManager.bluetoothService.ShowEnableBluetoothDialogAsync();
        //TODO
    }

    private async void OnRequestGPSEnableDialog(object sender, EventArgs e)
    {
        bool isActive = SpatialManager.CheckIfGpsIsEnabled();
        if (isActive) return; // won't do anything already active
        bool result = await SpatialManager.ShowEnableGPSDialogAsync();
    }

    private async void OnRequestBluetoothPermissionsDialog(object sender, EventArgs e)
    {
        bool granted = BluetoothPermissions.CheckStatus();
        if (granted) return; // won't do anything if we already got permissions;
        //TODO
    }

    private async void OnRequestGPSPermissionDialog(object sender, EventArgs e)
    {
        bool granted = await SpatialManager.IsLocationPermissionGrantedAsync();
        if (granted) return; // won't do anything if we already got permission;
        //TODO
    }

    public void OnTransmissionFailed(string msg)
    {
        FinishRecordingButton.Text = msg;
        FinishRecordingButton.IsEnabled = true;
        //TODO: add Delay til returning to other UI
    }
    public void OnTransmissionSuccess(string msg)
    {
        FinishRecordingButton.Text = "Transmission successful!";
        OverpassModule.lastFetchWasSuccess = false;
        OverpassModule.lastFetchWasSuccessButNoResults = false;
        OverpassModule.everFetchedLocations = false;

        Application.Current.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(4), () =>
        {
            SwitchToStandardUI();
        });
        //Submitting seems to work but Lambda doesnt write to DB so maybe message not correct yet? Compare with android message        
    }

    public void UpdateLocationPicker()
    {        
        Locations = OverpassModule.LocationData;
        if (Locations.Count == 0)
        {
            return;
        }
        LocationPicker.ItemsSource = null;
        LocationPicker.Items.Clear();
        LocationPicker.ItemsSource = Locations;


        if (Locations.Count > 0)
        {
            LocationPicker.SelectedItem = Locations[0];
        }            
    }

    private void OnSliderStartValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Update the label with the new slider value
        //sliderStartValueLabel.Text = "Remove first " + $"{(int)Math.Floor(e.NewValue)}";
        //UpdateFinishRecordingButton();
        startTrimSliderHasBeenUsed= true;
        //UpdateLineChart();
    }

    private void OnSliderEndValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Update the label with the new slider value
        //sliderEndValueLabel.Text = "Remove last " + $"{(int)Math.Floor(e.NewValue)}";
        //UpdateFinishRecordingButton();
        endTrimSliderHasBeenUsed = true;
        if(endTrimSlider.Value < endTrimSlider.Maximum)
        {
            endtrimSliderIsAtmax = false;
        }
        //UpdateLineChart();
    }

    private void OnEditorFocused(object sender, EventArgs e)
    {

    }

    private void OnEditorUnfocused(object sender, EventArgs e)
    {

    }

    #region
    public void Stop()
    {
        _timer.Dispose();
    }
    #endregion

    private void CheckBoxVentilation_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {        
        hasVentilationSystem= CheckBoxVentilation.IsChecked;
    }

    private void CheckBoxDoorsWindows_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        hasOpenWindowsDoors = CheckBoxDoorsWindows.IsChecked;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        lineChartView.WidthRequest = 0.9 * width;
        lineChartView.HeightRequest = 0.25 * height;
        startTrimSlider.WidthRequest = 0.4 * width;
        endTrimSlider.WidthRequest = 0.4 * width;
    }

    private void OnLocationLabelTapped(object sender, TappedEventArgs e)
    {
        hideLocation = !hideLocation;
    }

    private void OnCO2MonitorPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (firstInit) return;
        BluetoothManager.discoveredDevices = null;
        string picked = CO2MonitorPicker.SelectedItem.ToString();
        int index = CO2MonitorPicker.SelectedIndex;

        Preferences.Set(SelectedMonitorPreferenceKey, index);
        
        if (picked != null)
        {
            if(picked == "Aranet")
            {
                monitorType = CO2MonitorType.Aranet4;
            }
            else if(picked == "Airvalent")
            {
                monitorType = CO2MonitorType.Airvalent;
            }
            else if(picked == "Inkbird IAM-T1")
            {
                monitorType = CO2MonitorType.InkbirdIAMT1;
            }
        }        
    }

    private void LoadMonitorType()
    {
        if (Preferences.ContainsKey(SelectedMonitorPreferenceKey))
        {
            int savedIndex = Preferences.Get(SelectedMonitorPreferenceKey, -1);
            if (savedIndex != -1 && savedIndex < CO2MonitorPicker.Items.Count)
            {
                CO2MonitorPicker.SelectedIndex = savedIndex;

                if (CO2MonitorPicker.SelectedItem.ToString() == "Aranet")
                {
                    monitorType = CO2MonitorType.Aranet4;
                }
                else if (CO2MonitorPicker.SelectedItem.ToString() == "Airvalent")
                {
                    monitorType = CO2MonitorType.Airvalent;
                }
                else if (CO2MonitorPicker.SelectedItem.ToString() == "Inkbird IAM-T1")
                {
                    monitorType = CO2MonitorType.InkbirdIAMT1;
                }
            }
        }
        else
        {
            CO2MonitorPicker.SelectedIndex = 0; // Set default selected item if no preference is stored
            monitorType = CO2MonitorType.Aranet4;
        }
    }

    public string GetNotesEditorText()
    {
        if (NotesEditor.Text == null) return string.Empty;
        return NotesEditor.Text;
    }
}

