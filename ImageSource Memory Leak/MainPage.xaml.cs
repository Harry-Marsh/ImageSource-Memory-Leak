using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace ImageSource_Memory_Leak
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var viewModel = (MainPageViewModel)BindingContext;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MainPageViewModel.ChartBitmap))
                {
                    ChartView.InvalidateSurface(); // Redraw the canvas
                }
            };
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var viewModel = (MainPageViewModel)BindingContext;
            if (viewModel.ChartBitmap != null)
            {
                var canvas = e.Surface.Canvas;
                canvas.Clear(SKColors.White);
                var destRect = new SKRect(0, 0, e.Info.Width, e.Info.Height);
                canvas.DrawBitmap(viewModel.ChartBitmap, destRect);
            }
        }
    }
}
