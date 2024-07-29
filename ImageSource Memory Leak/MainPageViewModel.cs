using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using Microcharts;

namespace ImageSource_Memory_Leak
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly Random _random;
        private readonly DateTime _startTime;

        public MainPageViewModel()
        {
            _random = new Random();
            _startTime = DateTime.Now;

            // Initialize the chart with some default data
            Chart = CreateInitialChart();

            // Start polling for new values
            PollValuesAsync();
        }

        private LineChart CreateInitialChart()
        {
            var entries = GenerateRandomBpmData();
            return new LineChart
            {
                Entries = entries,
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle, // Display points on the chart
                LabelTextSize = 20, // Adjusted for better visibility
                LineSize = 4,
                BackgroundColor = SKColors.Transparent
            };
        }

        private List<ChartEntry> GenerateRandomBpmData()
        {
            var entries = new List<ChartEntry>();
            
                float bpm = GenerateRandomBpm(); // Generate random BPM value
                var elapsedTime = DateTime.Now;
                var label = elapsedTime.ToString(@"mm\:ss");

                entries.Add(new ChartEntry(bpm)
                {
                    Color = SKColor.Parse("#FF1493"), // DeepPink color
                    Label = label,
                    ValueLabel = bpm.ToString()
                    
                });

            return entries;
        }

        private float GenerateRandomBpm()
        {
            return (float)_random.Next(60, 120); // Random BPM between 60 and 120
        }

        public LineChart Chart { get; private set; }

        private async Task PollValuesAsync()
        {
            var entries = Chart.Entries.ToList(); // Get current entries

            while (true)
            {
                await Task.Delay(1000); // Delay for 1 second

                try
                {
                    // Generate new random BPM value
                    float bpm = GenerateRandomBpm();
                    var elapsedTime = DateTime.Now;
                    var label = elapsedTime.ToString(@"mm\:ss");

                    // Add new data point
                    entries.Add(new ChartEntry(bpm)
                    {
                        Color = SKColor.Parse("#FF1493"), // DeepPink color
                        Label = label,
                        ValueLabel = bpm.ToString()
                    });

                    // Update the chart
                    Chart = new LineChart
                    {
                        Entries = entries,
                        LineMode = LineMode.Straight,
                        PointMode = PointMode.Circle,
                        LabelTextSize = 20, // Adjusted for better visibility
                        LineSize = 4,
                        BackgroundColor = SKColors.Transparent,
                        IsAnimated = false,
                    };

                    // Notify that the Chart property has changed
                    OnPropertyChanged(nameof(Chart));
                }
                catch (Exception ex)
                {
                    // Handle or log the exception as needed
                    Console.WriteLine($"Error during timer event: {ex.Message}");
                }
            }
        }
    }
}
