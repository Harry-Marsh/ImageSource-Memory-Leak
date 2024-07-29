using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;
using Microcharts;

namespace ImageSource_Memory_Leak
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly Random _random;

        public MainPageViewModel()
        {
            _random = new Random();
            MyChart = CreateInitialChart();
            PollValuesAsync();
        }

        private LineChart CreateInitialChart()
        {
            return new LineChart
            {
                Entries = GenerateRandomBpmData(),
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle,
                LabelTextSize = 20,
                LineSize = 4,
                BackgroundColor = SKColors.Transparent
            };
        }

        private List<ChartEntry> GenerateRandomBpmData()
        {
            float bpm = GenerateRandomBpm();
            string label = DateTime.Now.ToString(@"mm\:ss");

            return new List<ChartEntry>
            {
                new ChartEntry(bpm)
                {
                    Color = SKColor.Parse("#FF1493"),
                    Label = label,
                    ValueLabel = bpm.ToString()
                }
            };
        }

        private float GenerateRandomBpm() => _random.Next(60, 120);

        [ObservableProperty]
        private LineChart myChart;

        private async Task PollValuesAsync()
        {
            var entries = MyChart.Entries.ToList();

            while (true)
            {
                await Task.Delay(1000);

                float bpm = GenerateRandomBpm();
                string label = DateTime.Now.ToString(@"mm\:ss");

                entries.Add(new ChartEntry(bpm)
                {
                    Color = SKColor.Parse("#FF1493"),
                    Label = label,
                    ValueLabel = bpm.ToString()
                });

                MyChart = new LineChart
                {
                    Entries = entries,
                    LineMode = LineMode.Straight,
                    PointMode = PointMode.Circle,
                    LabelTextSize = 20,
                    LineSize = 4,
                    BackgroundColor = SKColors.Transparent,
                    IsAnimated = false,
                };

            }
        }
    }
}
