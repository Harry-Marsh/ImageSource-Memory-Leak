using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System;
using System.IO;
using System.Threading;
using Microsoft.Maui.Controls;

//The Code in this view model is used to demonstrate an issue with the Bindable Context for Image Source.
//The view model creates a Byte[] adds it to an image source and uses the [Observable Property] to display on the screen.


namespace ImageSource_Memory_Leak
{
	public partial class MainPageViewModel : ObservableObject
	{
		private WeakReference<Timer> _timer; // Timer to periodically update the image

		public Timer Timer
		{
			get => _timer.TryGetTarget(out var timer) ? timer : null;
			set => _timer = new(value);

		}

		// Observable property to hold the current image source
		[ObservableProperty]
		private ImageSource _cameraImage;

		// Field to store the frame data byte array
		private byte[] _frameData;

		public MainPageViewModel()
		{
			// Start updating the image when the view model is instantiated
			StartImageUpdate();
		}

		private void StartImageUpdate()
		{
			// Initialize and start the timer to update the image 60 times per second (1000ms / 60 ≈ 16.67ms)
			Timer = new Timer(UpdateImage, null, 0, 1000 / 60);
		}

		private void UpdateImage(object state)
		{
			// Generate random image byte array
			byte[] imageData = ImageGenerator.GenerateRandomImage();

			// Update the Image control on the main thread
			_frameData = imageData; // Store the frame data

			// Create a new StreamImageSource from the frame data
			var streamImageSource = new StreamImageSource
			{
				Stream = token =>
				{
					// Create a MemoryStream from the frame data
					MemoryStream memoryStream = new MemoryStream(_frameData);
					_frameData = null; // Clear the frame data after creating the stream to release memory
					return Task.FromResult<Stream>(memoryStream);
				}
			};

			// Assign the new StreamImageSource to the CameraImage property to update the UI
			CameraImage = streamImageSource;
		}
		// Destructor to clean up the timer when the view model is destroyed
		~MainPageViewModel()
		{

			// Some times when you navigate from the Mainpage, the breakpoint wouldn't be hitten
			// but if you take a snapshot there's no reference to this type... I belive that's expected
			// Because Finalizers are called from a specific thread and the debugger could get lost, or 
			// it's just a bug, who knows? ¯\(ツ)/¯
			Timer?.Dispose(); // Dispose the timer to stop updates
		}
	}

	public class ImageGenerator
	{
		/// <summary>
		/// Generates a random byte array representing a 500x500 pixel image in JPEG format.
		/// </summary>
		/// <returns>A byte array representing the image.</returns>
		public static byte[] GenerateRandomImage()
		{
			int width = 500; // Width of the image
			int height = 500; // Height of the image

			// Create an empty bitmap with SkiaSharp
			using (var bitmap = new SKBitmap(width, height))
			{
				var random = new Random(); // Random number generator for pixel colors

				// Generate random pixels for the bitmap
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						// Create a random color
						var randomColor = new SKColor(
							(byte)random.Next(256), // Red component (0-255)
							(byte)random.Next(256), // Green component (0-255)
							(byte)random.Next(256)  // Blue component (0-255)
						);

						// Set the pixel color in the bitmap
						bitmap.SetPixel(x, y, randomColor);
					}
				}

				// Create an image from the bitmap and encode it to JPEG format
				using (var image = SKImage.FromBitmap(bitmap))
				using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
				{
					// Return the encoded image as a byte array
					return data.ToArray();
				}
			}
		}
	}
}
