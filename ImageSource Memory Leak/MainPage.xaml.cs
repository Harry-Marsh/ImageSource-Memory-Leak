using SkiaSharp;
using System;
using System.IO;
using System.Threading;

namespace ImageSource_Memory_Leak
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
		{
			FirstPage.FullGC();
			FirstPage.FullGC();
		}

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is MainPageViewModel viewModel)
            {
                FullGC();
                viewModel.Dispose();
            }
        }

        public static void FullGC()
        {

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

	sealed class FirstPage : ContentPage
	{
		public FirstPage()
		{
			var btn = new Button
			{
				Text = "Navigate",
				VerticalOptions = LayoutOptions.Center
			};

			btn.Clicked += async (_, __) =>
			{
				FullGC();
				await Shell.Current.Navigation.PushAsync(new MainPage());
			};

			Content = btn;
		}



        public static void FullGC()
		{

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}
}


