using Mntone.SvgForXaml;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SvgImageControlSample
{
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/test4.svg"));
			this.InitializeAsync(file);
		}

		private void OnUnloaded(object sender, RoutedEventArgs e) => this.SvgImageControl.SafeUnload();

		private async void InitializeAsync(StorageFile file)
		{
			await this.SvgImageControl.LoadFileAsync(file);
		}

		private async void OnOpenClick(object sender, RoutedEventArgs e)
		{
			var picker = new FileOpenPicker();
			picker.ViewMode = PickerViewMode.List;
			picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			picker.FileTypeFilter.Add(".svg");

			var file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				this.PathTextBox.Text = file.Path;
				this.InitializeAsync(file);
			}
		}

		private async void OnSaveClick(object sender, RoutedEventArgs e)
		{
			var picker = new FileSavePicker();
			picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			picker.DefaultFileExtension = ".png";
			picker.FileTypeChoices.Add(new KeyValuePair<string, IList<string>>("Bitmap image", new[] { ".bmp" }.ToList()));
			picker.FileTypeChoices.Add(new KeyValuePair<string, IList<string>>("Png image", new[] { ".png" }.ToList()));
			picker.FileTypeChoices.Add(new KeyValuePair<string, IList<string>>("Jpeg image", new[] { ".jpg", ".jpe", ".jpeg" }.ToList()));
			picker.FileTypeChoices.Add(new KeyValuePair<string, IList<string>>("Gif image", new[] { ".gif" }.ToList()));

			var file = await picker.PickSaveFileAsync();
			if (file != null)
			{
				SvgImageRendererFileFormat format;
				switch (file.FileType)
				{
					case ".bmp":
						format = SvgImageRendererFileFormat.Bitmap;
						break;

					case ".png":
						format = SvgImageRendererFileFormat.Png;
						break;

					case ".jpg":
					case ".jpe":
					case ".jpeg":
						format = SvgImageRendererFileFormat.Jpeg;
						break;

					case ".gif":
						format = SvgImageRendererFileFormat.Gif;
						break;

					default:
						return;
				}

				var content = this.SvgImageControl.Content;
				await SvgImageRenderer.RendererImageAsync(file, new SvgImageRendererSettings()
				{
					Document = content,
					Format = format,
					Scaling = 200,
				});
			}
		}
	}
}