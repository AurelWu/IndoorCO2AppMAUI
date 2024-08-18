using CommunityToolkit.Maui;
using IndoorCO2App_Android;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace IndoorCO2App_Android.Controls
{


    public class LineChartView : GraphicsView
    {
        private readonly LineChartDrawable _drawable;

        public LineChartView()
        {
            _drawable = new LineChartDrawable();
            Drawable = _drawable;
            UpdateThemeColor();
        }

        internal void SetData(List<SensorData> newData)
        {
            int[] ints = new int[newData.Count];
            for (int i = 0; i < newData.Count; i++)
            {
                ints[i] = newData[i].CO2ppm;
            }
            _drawable.Data = ints;
            Invalidate(); // Force the view to redraw with the new data
        }

        private void UpdateThemeColor()
        {
            // Retrieve the theme color from the resources
            ResourceDictionary colorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault() as ResourceDictionary;
            if (colorResource != null && colorResource.TryGetValue("ThemeLineColor", out var colorValue) && colorValue is AppThemeColor themeColor)
            {
                AppTheme currentTheme = Application.Current.RequestedTheme;
                Color lineColor = currentTheme == AppTheme.Light ? themeColor.Light : themeColor.Dark;
                _drawable.LineColor = lineColor;
            }
            else
            {
                // Fallback color if not found
                _drawable.LineColor = Colors.Gray;
            }
        }

        private class LineChartDrawable : IDrawable
        {
            public int[] Data { get; set; } = new int[0];
            public Color LineColor { get; set; } = Colors.Gray;

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                int width = (int)dirtyRect.Width;
                int height = (int)dirtyRect.Height;
                int paddingLeft = 25;
                int paddingRight = 25;
                int paddingTop = 0;
                int paddingBottom = 0;

                // Calculate the usable area after considering padding
                int usableWidth = width - paddingLeft - paddingRight;
                int usableHeight = height - paddingTop - paddingBottom;

                canvas.FillColor = LineColor;
                //canvas.FillRectangle(0, 0, width, height);

                // Calculate scaling factors
                float xScale = usableWidth / (float)(Data.Length - 1);
                float maxValue = GetMaxDataValue();
                float minValue = 300;
                float yScale = usableHeight / (maxValue - minValue);
                // Draw X-axis tick marks
                canvas.StrokeColor = Colors.Gray;

                canvas.StrokeSize = 2;
                for (int i = 0; i < Data.Length; i++)
                {
                    float x = paddingLeft + i * xScale;
                    if (i % 5 == 0)
                    {
                        canvas.DrawLine(x, height - paddingBottom, x, height - paddingBottom - 10); // Bigger tick
                    }
                    else
                    {
                        canvas.DrawLine(x, height - paddingBottom, x, height - paddingBottom - 5); // Regular tick
                    }
                }

                // Draw Y-axis tick marks and labels
                canvas.StrokeColor = LineColor;
                canvas.FontColor = LineColor;
                canvas.FontSize = 8;
                canvas.StrokeSize = 2;
                for (int i = 400; i <= maxValue; i += 400)
                {
                    float y = height - paddingBottom - (i - minValue) * yScale;
                    canvas.DrawLine(paddingLeft, y, paddingLeft + 10, y); // Tick mark
                    canvas.DrawString(i.ToString(), paddingLeft + 12, y - 5, 300, 10, HorizontalAlignment.Left, VerticalAlignment.Center);
                }



                if (Data.Length > 0)
                {
                    // Draw data points
                    canvas.StrokeColor = LineColor;
                    canvas.StrokeSize = 2;
                    float prevX = paddingLeft;
                    float prevY = height - paddingBottom - (Data[0] - minValue) * yScale;

                    for (int i = 0; i < Data.Length; i++)
                    {
                        if (i < MainPage.startTrimSliderValue + 1 || i >= MainPage.endTrimSliderValue)
                        {
                            canvas.StrokeDashPattern = new float[] { 1, 1 };
                            canvas.StrokeColor = Color.FromRgb(155, 155, 155);
                            canvas.FillColor = Color.FromRgb(155, 155, 155);
                        }
                        else
                        {
                            canvas.StrokeDashPattern = new float[] { 1, 0 };
                            canvas.StrokeColor = LineColor;
                            canvas.FillColor = LineColor;
                        }
                        float x = paddingLeft + i * xScale;
                        float y = height - paddingBottom - (Data[i] - minValue) * yScale;
                        if (i > 0)
                        {
                            canvas.DrawLine(prevX, prevY, x, y);
                        }
                        prevX = x;
                        prevY = y;

                        if (i < MainPage.startTrimSliderValue || i >= MainPage.endTrimSliderValue)
                        {
                            canvas.FillColor = Color.FromRgb(155, 155, 155);
                        }
                        else
                        {
                            canvas.FillColor = LineColor;
                        }

                        // Draw circles on data points
                        if (Data.Length <= 25)
                        {
                            canvas.FillCircle(x, y, 5); // Adjust the radius as needed for the circle
                        }
                        else if (Data.Length <= 50)
                        {
                            canvas.FillCircle(x, y, 3); // Adjust the radius as needed for the circle
                        }
                        else
                        {
                            canvas.FillCircle(x, y, 2); // Adjust the radius as needed for the circle
                        }
                    }
                }

                // Draw X and Y axes
                canvas.StrokeColor = LineColor;
                canvas.StrokeDashPattern = new float[] { 1, 0 };
                canvas.StrokeSize = 2;
                canvas.DrawLine(paddingLeft, paddingTop, paddingLeft, height - paddingBottom); // Y-axis
                canvas.DrawLine(paddingLeft, height - paddingBottom, width - paddingRight, height - paddingBottom); // X-axis






            }

            // Helper method to get the maximum value in the data array
            private float GetMaxDataValue()
            {
                if (Data.Length == 0)
                {
                    return 1000;
                }

                int max = Data[0];
                foreach (int value in Data)
                {
                    if (value > max)
                    {
                        max = value;
                    }
                }

                // Round up to the next multiple of 400
                return ((((max + 399) / 400) * 400) + 50);
            }
        }
    }
}