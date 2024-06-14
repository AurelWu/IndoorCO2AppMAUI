using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui;

namespace IndoorCO2App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{
    public const string CHANNEL_ID = "com.companyname.appname.channel";

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        CreateNotificationChannel();
        StartForegroundService();
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channelName = "Foreground Service Channel";
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default)
            {
                Description = "Channel for foreground service"
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }

    private void StartForegroundService()
    {
        var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            Android.App.Application.Context.StartForegroundService(intent);
        }
        else
        {
            Android.App.Application.Context.StartService(intent);
        }
    }
}
