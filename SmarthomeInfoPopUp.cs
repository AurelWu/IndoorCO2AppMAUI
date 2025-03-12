using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace IndoorCO2App_Android
{
    class SmarthomeInfoPopUp : Popup
    {   
        public SmarthomeInfoPopUp()
        {

            var titleLabel = new Label
            {
                Text = "Enabling Smart Home Integration",
                TextColor = Colors.Black,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            var description1 = new Label
            {
                Text = "To read the data from your Aranet4, Smart Home Integration needs to be enabled. \r\n 1) Open the official Aranet Home App (install from App Store if not installed)",
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            var image1 = new Image
            {
                Source = "aranetlogo.png", 
                HeightRequest = 64,
                WidthRequest = 64,
                //Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center
            };

            var description2 = new Label
            {
                Text = "2) Press the Gears Icon in top right of the sensor",
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
                Text = "3) move to Smart Home Integration Slider to the right ",
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center
            };

            var image3 = new Image
            {
                Source = "smarthome_slider.png", 
                HeightRequest = 200,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center
            };


            var closeButton = new Button
            {
                Text = "Close",
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
