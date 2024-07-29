using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ImageSource_Memory_Leak
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly Random _random;
        private readonly List<(DateTime Time, float Bpm)> _dataPoints;
        private readonly Timer _timer;

        public MainPageViewModel()
        {
            _random = new Random();
            _dataPoints = new List<(DateTime Time, float Bpm)>();
            _timer = new Timer(OnTimerTick, null, 1000, 1000); // 1-second interval
        }

        [ObservableProperty]
        private SKBitmap chartBitmap;

        private float GenerateRandomBpm() => _random.Next(60, 120);

        private void OnTimerTick(object state)
        {
            float bpm = GenerateRandomBpm();
            DateTime now = DateTime.Now;

            _dataPoints.Add((now, bpm));
            if (_dataPoints.Count > 10) _dataPoints.RemoveAt(0); // Keep last 10 data points

            UpdateChart();
        }

        private void UpdateChart()
        {
            int width = 800;
            int height = 400;

            using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                if (_dataPoints.Count > 1)
                {
                    var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Stroke,
                        Color = SKColors.DeepPink,
                        StrokeWidth = 4
                    };

                    var path = new SKPath();
                    var firstPoint = _dataPoints.First();
                    var startX = 0;
                    var scaleX = width / (float)(_dataPoints.Count - 1);
                    var scaleY = height / 60f;

                    path.MoveTo(startX, height - firstPoint.Bpm * scaleY);

                    for (int i = 1; i < _dataPoints.Count; i++)
                    {
                        var point = _dataPoints[i];
                        var x = i * scaleX;
                        var y = height - point.Bpm * scaleY;
                        path.LineTo(x, y);
                    }

                    canvas.DrawPath(path, paint);
                }

                ChartBitmap = surface.Snapshot().ToBitmap();
            }
        }
    }

    public static class SKSurfaceExtensions
    {
        public static SKBitmap ToBitmap(this SKImage image)
        {
            var bitmap = new SKBitmap(image.Width, image.Height);
            image.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.Info.RowBytes, 0, 0);
            return bitmap;
        }
    }
}
