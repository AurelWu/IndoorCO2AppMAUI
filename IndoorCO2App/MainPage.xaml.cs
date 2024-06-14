
//TODO: check if works on Android
//TODO: Overpass Module
//TODO: iphone Developer Account ?!?
//TODO: Layout
//TODO: Button Functionality

using Microsoft.Maui;
using System.Diagnostics;

namespace IndoorCO2App;

public partial class MainPage : ContentPage
{
	int count = 0;
    private readonly PeriodicTimer _timer;

    private DateTime timeOfLastGPSUpdate = DateTime.MinValue;
    internal List<LocationData> Locations { get; set; }
    LocationData selectedLocation;
    public static MainPage MainPageSingleton;
    public static IWakeLockService wakeLockService;

    //public const string CHANNEL_ID = "com.companyname.indoorco2app.channel";

    public MainPage()
	{
        MainPageSingleton = this;
        Locations = new List<LocationData>();        
        //TODO => Overpass search range based on radio button instead of hardcoded 100
        //TODO: => record data when recording mode
        //TODO: => draw Linechart when recording
        //TODO: => add cancel button when recording (with confirmation)
        //TODO: => add submit Data Button (with info if success or not stuff)
        //TODO: => Data submission

        InitializeComponent();
        BluetoothManager.Init();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

#if ANDROID
        wakeLockService = new WakeLockServiceAndroid();
#elif IOS
        wakeLockService = new WakeLockServiceiOS();                
#elif WINDOWS
        wakeLockService = new WakeLockServiceWindows();
#endif


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
        UpdateLocationsButton.MinimumWidthRequest = buttonWidth70Percent;
        OpenMapButton.MinimumWidthRequest = buttonWidth70Percent;
        OpenImprintButton.MinimumWidthRequest = buttonWidth70Percent;
        FinishRecordingButton.MinimumWidthRequest = buttonWidth60Percent;
        FinishRecordingButton.MaximumWidthRequest = buttonWidth60Percent;
        RequestCancelRecordingButton.MinimumWidthRequest = buttonWidth25Percent;
        RequestCancelRecordingButton.MaximumWidthRequest = buttonWidth25Percent;
        ConfirmCancelRecordingButton.MinimumWidthRequest = buttonWidth25Percent;
        ConfirmCancelRecordingButton.MaximumWidthRequest = buttonWidth25Percent;
    }

    private void SwitchToRecordingUI()
    {
        wakeLockService.AcquireWakeLock();
        //StackRangeTrimmer.IsVisible = true;        
        FinishRecordingButton.Text = "Submit Data";
        FinishRecordingButton.IsEnabled = true;
        FinishRecordingButton.IsVisible = true;
        RequestCancelRecordingButton.IsVisible = true;
        ConfirmCancelRecordingButton.IsVisible = false;
        RecordedDataLabel.IsVisible = true; //RecordedDataLabel is temporary instead of lineChart
        LocationLabelRecording.IsVisible = true;
        LocationLabel.IsVisible = false;
        SearchRangeStackLayout.IsVisible = false;
        LocationStackLayout.IsVisible = false;
        StartRecordingButton.IsVisible = false;
        OpenImprintButton.IsVisible = false;
        OpenMapButton.IsVisible = false;
        UpdateLocationsButton.IsVisible = false;
    }

    private void SwitchToStandardUI() 
    {
        wakeLockService.ReleaseWakeLock();
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
        OpenImprintButton.IsVisible = true;
        OpenMapButton.IsVisible = true;
        UpdateLocationsButton.IsVisible = true;
    }

    private void OnUpdateLocationsClicked(object sender, EventArgs e)
    {
        SemanticScreenReader.Announce(UpdateLocationsButton.Text);
        OverpassModule.FetchNearbyBuildingsAsync(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, 100,this); //TODO => 
    }

    private void OnStartRecordingClicked(object sender, EventArgs e)
    {    	    
    	SemanticScreenReader.Announce(StartRecordingButton.Text);

        //Console.WriteLine(LocationPicker);
        if(LocationPicker != null && Locations.Count>0) 
        {
            if(LocationPicker.SelectedItem != null)
            {
                selectedLocation = (LocationData)LocationPicker.SelectedItem;
            }
            SwitchToRecordingUI();
            BluetoothManager.StartNewRecording(selectedLocation, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }        
    }

    private void OnFinishRecordingClicked(object sender, EventArgs e)
    {
        FinishRecordingButton.Text = "Submitting Data";
        FinishRecordingButton.IsEnabled = false; ;
        BluetoothManager.FinishRecording();
    }
    
    private void OnRequestCancelRecordingClicked(object sender, EventArgs e)
    {
        RequestCancelRecordingButton.IsVisible = false;
        ConfirmCancelRecordingButton.IsVisible = true;
    }

    private void OnConfirmCancelRecordingClicked(object sender, EventArgs e)
    {
        BluetoothManager.StopRecording();
        SwitchToStandardUI();
    }

    private void OnShowMapInBrowserClicked(object sender, EventArgs e)
    {
        SemanticScreenReader.Announce(OpenMapButton.Text);
    }

    private void OnImprintClicked(object sender, EventArgs e)
    {
        SemanticScreenReader.Announce(OpenImprintButton.Text);
    }


    private async void Update()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync())
            {
                UpdateUI();
                StatusLabel.Text = "CO2 Levels: " + BluetoothManager.currentCO2Reading + " | " + DateTime.Now;
                //BluetoothManager.Update();

                //non-UI Stuff now done in foreground Service
                //if (DateTime.Now - timeOfLastGPSUpdate > TimeSpan.FromSeconds(15))
                //{
                //    SpatialManager.UpdateLocation();
                //    timeOfLastGPSUpdate = DateTime.Now;
                //}

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
        UpdateRecordedDataLabel();
    }

    private async void UpdateGPSPermissionButton()
    {
        bool granted = await SpatialManager.IsLocationPermissionGrantedAsync();
        if(granted)
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
        bool isActive = SpatialManager.CheckIfGpsIsEnabled();
        if(isActive)
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
        bool isActive = BluetoothManager.bluetoothService.IsBluetoothEnabled();
        if(isActive)
        {
            ButtonBluetoothStatus.BackgroundColor = Color.Parse("Green");
        }
        else
        {
            ButtonBluetoothStatus.BackgroundColor = Color.Parse("Red");
        }
    }

    private async void UpdateBluetoothPermissionsButton()
    {
        bool granted = await BluetoothPermissions.CheckBluetoothPermissionStatus();
        if(granted)
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
        LocationLabel.Text = "Lat: " + SpatialManager.currentLocation.Latitude.ToString("0.######") + " | Lon:" + SpatialManager.currentLocation.Longitude.ToString("0.######");
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
        string s = String.Join(" ", BluetoothManager.recordedData);
        RecordedDataLabel.Text = " recorded CO2-Values: " + s;
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
        bool granted = await BluetoothPermissions.CheckBluetoothPermissionStatus();
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
        Application.Current.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(4), () =>
        {
            SwitchToStandardUI();
        });
        //Submitting seems to work but Lambda doesnt write to DB so maybe message not correct yet? Compare with android message        
    }

    public void UpdateLocationPicker()
    {
        Locations = OverpassModule.LocationData;
        LocationPicker.ItemsSource = Locations;
        if (Locations.Count > 0)
        {
            LocationPicker.SelectedItem = Locations[0];
        }            
    }

    #region
    public void Stop()
    {
        _timer.Dispose();
    }
    #endregion
}

