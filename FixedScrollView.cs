#if IOS
using Microsoft.Maui.Platform;
using IndoorCO2App_Android.Platforms.iOS.Utils;
using UIKit;
using CoreGraphics;
#endif


namespace IndoorCO2App_Android.Controls
{
    public class FixedScrollView : ScrollView
    {
        public FixedScrollView()
        {
#if IOS
            UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShowing);
            UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHiding);
#endif
        }

#if IOS
        private void OnKeyboardShowing(object sender, UIKeyboardEventArgs args)
        {
            //if (Application.Current.MainPage is ContentPage page)
            {
                UIView control = this.ToPlatform(Handler.MauiContext).FindFirstResponder();
                //UIView rootUiView = page.Content.ToPlatform(Handler.MauiContext);
                UIView rootUiView = Application.Current.MainPage.ToPlatform(Handler.MauiContext);
                CGRect kbFrame = UIKeyboard.FrameEndFromNotification(args.Notification);
                double distance = control.GetOverlapDistance(rootUiView, kbFrame);
                if (distance > 0)
                {
                    Margin = new Thickness(Margin.Left, -distance, Margin.Right, distance);
                }
            }
        }
        private void OnKeyboardHiding(object sender, UIKeyboardEventArgs args)
        {
            Margin = new Thickness(Margin.Left, 0, Margin.Right, 0);
        }
#endif
    }
}

