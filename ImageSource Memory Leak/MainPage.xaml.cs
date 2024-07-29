using Microsoft.Maui.Controls;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Diagnostics;

namespace ImageSource_Memory_Leak
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var viewModel = new MainPageViewModel();
            BindingContext = viewModel;

            // Subscribe to property change notifications
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainPageViewModel.MyChart))
            {
                // Force the chart view to refresh by resetting the binding context
                var chartView = this.FindByName<ChartView>("ChartView");
                chartView.BindingContext = null;
                chartView.BindingContext = BindingContext;
            }

        }
    }
}