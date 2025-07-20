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
    partial class UpdateAranetPopUp : Popup
    {   
        public UpdateAranetPopUp()
        {

            var titleLabel = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.FirmwareGuideHeader)),
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center
            };

            var description1 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.FirmwareGuideText1)),
                FontSize = 12,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center
            };

            var image1 = new Image
            {
                Source = "aranetlogo.png", // Make sure this exists in your resources
                HeightRequest = 64,
                WidthRequest = 64,
                //Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center
            };

            var description2 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.FirmwareGuideText2)),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            var image2 = new Image
            {
                Source = "smarthome_gears.png",
                HeightRequest = 125,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center
            };

            var description3 = new Label
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.FirmwareGuideText3)),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            var image3 = new Image
            {
                Source = "aranet_firmware.png", 
                HeightRequest = 200,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center
            };


            var closeButton = new Button
            {
                Text = LocalisationResourceManager.Instance.GetString(nameof(AppStrings.QuickGuidePopupCloseButton)),
                TextColor = Colors.Black,
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
                image1,
                description2,
                image2,
                description3,
                image3,
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
