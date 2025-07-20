using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using IndoorCO2App_Android;
using Microsoft.Maui.Controls;

namespace IndoorCO2App_Multiplatform
{

    partial class QuickGuidePopUp : Popup
    {
        public static string userManualURL = "https://indoorco2map.com/Manual.pdf";

        public QuickGuidePopUp()
        {

            var titleLabel = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupHeadline)),
                TextColor = Colors.Black,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            var description1 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupPart1)),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            //var image1 = new Image
            //{
            //    Source = "aranetlogo.png", // Make sure this exists in your resources
            //    HeightRequest = 64,
            //    WidthRequest = 64,
            //    //Aspect = Aspect.AspectFit,
            //    HorizontalOptions = LayoutOptions.Center
            //};

            var description2 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupPart2)),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            //var image2 = new Image
            //{
            //    Source = "smarthome_gears.png",
            //    HeightRequest = 125,
            //    Aspect = Aspect.AspectFit,
            //    HorizontalOptions = LayoutOptions.Center
            //};

            var description3 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupPart3)),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            //var image3 = new Image
            //{
            //    Source = "aranet_firmware.png", 
            //    HeightRequest = 200,
            //    Aspect = Aspect.AspectFit,
            //    HorizontalOptions = LayoutOptions.Center
            //};

            var description4 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupPart4)),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            var description5 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupPart5)),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            var description6 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupPart6)),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            var description7 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupPart7)),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            var description8 = new Label
            {
                Text = "\r\n (This Button will be moved to the bottom of the UI after first successful usage)",
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };


            var LinkToManualButton = new Button
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupLinkManual)),
                Command = new Command(async () => await Launcher.OpenAsync(userManualURL))
            };


            var closeButton = new Button
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupCloseButton)),
                Command = new Command(() => this.Close())
            };


            var popupContent = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 10,
                BackgroundColor = Color.FromArgb("#F0F8FF"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children =
        {
            titleLabel,
            description1,
            //image1,
            description2,
            //image2,
            description3,
            description4,
            description5,
            description6,
            description7,
            //image3,
            LinkToManualButton,
            closeButton
        }
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => this.Close(); // Close on tap anywhere
            popupContent.GestureRecognizers.Add(tapGestureRecognizer);

            Content = popupContent;
        }
    }
}
