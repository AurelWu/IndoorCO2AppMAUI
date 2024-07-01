using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IndoorCO2App
{
    public class RangeSlider : ContentView
    {
        private readonly static int TRACK_HEIGHT = 4;
        private readonly static int THUMB_WIDTH = 20;
        private readonly static int THUMB_RADIUS = THUMB_WIDTH / 2;
        private readonly static double DEFAULT_MIN_VALUE = 0.25;
        private readonly static double DEFAULT_MAX_VALUE = 0.75;
        private readonly static Color DEFAULT_RANGE_COLOR = Colors.LightBlue;
        private readonly static Color DEFAULT_OUT_OF_RANGE_COLOR = Colors.LightGray;
        private readonly static Color DEFAULT_THUMB_COLOR = Colors.White;

        private Grid LeftTrack = new Grid
        {
            HeightRequest = TRACK_HEIGHT,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = DEFAULT_OUT_OF_RANGE_COLOR
        };

        private Grid CenterTrack = new Grid
        {
            HeightRequest = TRACK_HEIGHT,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = DEFAULT_RANGE_COLOR
        };

        private Grid RightTrack = new Grid
        {
            HeightRequest = TRACK_HEIGHT,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = DEFAULT_OUT_OF_RANGE_COLOR
        };

        private Frame MinThumb = new Frame
        {
            CornerRadius = 10,
            BackgroundColor = DEFAULT_THUMB_COLOR,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            BorderColor = DEFAULT_RANGE_COLOR,
            WidthRequest = THUMB_WIDTH,
            HeightRequest = THUMB_WIDTH,
        };

        private Frame MaxThumb = new Frame
        {
            CornerRadius = 10,
            BackgroundColor = DEFAULT_THUMB_COLOR,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            BorderColor = DEFAULT_RANGE_COLOR,
            WidthRequest = THUMB_WIDTH,
            HeightRequest = THUMB_WIDTH,
        };

        public static readonly BindableProperty MinValueProperty =
            BindableProperty.Create(nameof(MinValue),
                                    typeof(double),
                                    typeof(RangeSlider),
                                    DEFAULT_MIN_VALUE,
                                    BindingMode.TwoWay);

        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        public static readonly BindableProperty MaxValueProperty =
            BindableProperty.Create(nameof(MaxValue),
                                    typeof(double),
                                    typeof(RangeSlider),
                                    DEFAULT_MAX_VALUE,
                                    BindingMode.TwoWay);

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public static readonly BindableProperty RangeColorProperty =
            BindableProperty.Create(nameof(RangeColor),
                                    typeof(Color),
                                    typeof(RangeSlider),
                                    DEFAULT_RANGE_COLOR,
                                    BindingMode.TwoWay,
                                    propertyChanged: (bindable, oldValue, newValue) =>
                                    {
                                        if (bindable is RangeSlider slider && newValue is Color color)
                                        {
                                            slider.CenterTrack.BackgroundColor = color;
                                            slider.MinThumb.BorderColor = color;
                                            slider.MaxThumb.BorderColor = color;
                                        }
                                    });

        public Color RangeColor
        {
            get => (Color)GetValue(RangeColorProperty);
            set => SetValue(RangeColorProperty, value);
        }

        public static readonly BindableProperty ThumbsColorProperty =
            BindableProperty.Create(nameof(ThumbsColor),
                                    typeof(Color),
                                    typeof(RangeSlider),
                                    DEFAULT_THUMB_COLOR,
                                    BindingMode.TwoWay,
                                    propertyChanged: (bindable, oldValue, newValue) =>
                                    {
                                        if (bindable is RangeSlider slider && newValue is Color color)
                                        {
                                            slider.MinThumb.BackgroundColor = color;
                                            slider.MaxThumb.BackgroundColor = color;
                                        }
                                    });

        public Color ThumbsColor
        {
            get => (Color)GetValue(ThumbsColorProperty);
            set => SetValue(ThumbsColorProperty, value);
        }

        public static readonly BindableProperty OutOfRangeColorProperty =
            BindableProperty.Create(nameof(OutOfRangeColor),
                                    typeof(Color),
                                    typeof(RangeSlider),
                                    DEFAULT_OUT_OF_RANGE_COLOR,
                                    BindingMode.TwoWay,
                                    propertyChanged: (bindable, oldValue, newValue) =>
                                    {
                                        if (bindable is RangeSlider slider && newValue is Color color)
                                        {
                                            slider.LeftTrack.BackgroundColor = color;
                                            slider.RightTrack.BackgroundColor = color;
                                        }
                                    });

        public Color OutOfRangeColor
        {
            get => (Color)GetValue(OutOfRangeColorProperty);
            set => SetValue(OutOfRangeColorProperty, value);
        }

        public RangeSlider()
        {
            Content = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };

            ((Grid)Content).Children.Add(LeftTrack);
            ((Grid)Content).Children.Add(CenterTrack);
            ((Grid)Content).Children.Add(RightTrack);
            ((Grid)Content).Children.Add(MinThumb);
            ((Grid)Content).Children.Add(MaxThumb);

            Grid.SetRow(LeftTrack, 0);
            Grid.SetRow(CenterTrack, 0);
            Grid.SetRow(RightTrack, 0);
            Grid.SetRow(MinThumb, 1);
            Grid.SetRow(MaxThumb, 1);

            var minPanGesture = new PanGestureRecognizer();
            minPanGesture.PanUpdated += (sender, args) =>
            {
                if (args.StatusType == GestureStatus.Running)
                {
                    MinThumb.TranslateTo(MinThumb.TranslationX + args.TotalX, 0);
                    UpdateMinMaxValues();
                }
            };
            MinThumb.GestureRecognizers.Add(minPanGesture);

            var maxPanGesture = new PanGestureRecognizer();
            maxPanGesture.PanUpdated += (sender, args) =>
            {
                if (args.StatusType == GestureStatus.Running)
                {
                    MaxThumb.TranslateTo(MaxThumb.TranslationX + args.TotalX, 0);
                    UpdateMinMaxValues();
                }
            };
            MaxThumb.GestureRecognizers.Add(maxPanGesture);

            MinThumb.SizeChanged += (sender, args) =>
            {
                TranslateThumbRel(MinThumb, 0.5, DEFAULT_MIN_VALUE);
                TranslateThumbRel(MaxThumb, 0.5, DEFAULT_MAX_VALUE);
            };

            MaxThumb.SizeChanged += (sender, args) =>
            {
                TranslateThumbRel(MinThumb, 0.5, DEFAULT_MIN_VALUE);
                TranslateThumbRel(MaxThumb, 0.5, DEFAULT_MAX_VALUE);
            };
        }

        private void UpdateMinMaxValues()
        {
            SetValue(MinValueProperty, (EffectiveMinThumbX + THUMB_RADIUS) / Width);
            SetValue(MaxValueProperty, (EffectiveMaxThumbX + THUMB_RADIUS) / Width);
        }

        private void TranslateThumbRel(View thumb, double oldValue, double newValue)
        {
            var relativeDelta = newValue - Math.Max(0, Math.Min(1, oldValue));
            var absoluteDelta = relativeDelta * Width;
            thumb.TranslateTo(thumb.TranslationX + absoluteDelta, 0);
            UpdateTracks();
        }

        private void UpdateTracks()
        {
            var leftSpace = (int)Math.Round(MinValue * 100, 0);
            var centerSpace = (int)Math.Round((MaxValue - MinValue) * 100);
            var rightSpace = (int)Math.Round((1 - MaxValue) * 100);

            LeftTrack.WidthRequest = leftSpace;
            CenterTrack.WidthRequest = centerSpace;
            RightTrack.WidthRequest = rightSpace;
        }

        private double EffectiveMinThumbX => MinThumb.X + MinThumb.TranslationX;
        private double EffectiveMaxThumbX => MaxThumb.X + MaxThumb.TranslationX;
    }
}
