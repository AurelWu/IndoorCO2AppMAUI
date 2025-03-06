using Foundation;
using UIKit;
using System;
using System.IO;
using Microsoft.Maui.Storage;
using System.Runtime.InteropServices;


namespace IndoorCO2App_Multiplatform
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        //public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        //{
        //    // Catch .NET unhandled exceptions
        //    AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        //    {
        //        Logger.LogError("AppDomain Exception", e.ExceptionObject as Exception);
        //    };
        //
        //    // Catch Objective-C (native iOS) crashes
        //    NSSetUncaughtExceptionHandler((exception) =>
        //    {
        //        string reason = exception?.Reason ?? "Unknown";
        //        Logger.LogError("iOS Native Crash", new Exception(reason));
        //    });
        //
        //    return base.FinishedLaunching(app, options);
        //}
        //[System.Runtime.InteropServices.DllImport("__Internal")]
        //private static extern void NSSetUncaughtExceptionHandler(Action<NSException> handler);
    }
    
}
