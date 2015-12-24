using Mntone.SvgForXaml;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Mntone.SvgForXaml.DemoApp
{
	public sealed partial class MainPage : Page
	{
#if WINDOWS_PHONE_APP
		private MainPageArgs? _args = null;
#endif

		public MainPage()
		{
			this.InitializeComponent();
		}

#if WINDOWS_PHONE_APP
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Parameter as string))
			{
				this._args = (MainPageArgs)e.Parameter;
			}
		}
#endif

		private async void OnPageLoaded(object sender, RoutedEventArgs e)
		{
			StorageFile file;
#if WINDOWS_PHONE_APP
			if (this._args.HasValue && this._args.Value.Operation == MainPageOperation.Open)
			{
				file = await StorageFile.GetFileFromPathAsync(this._args.Value.Path);
			}
			else
#endif
			{
				file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/test1.svg"));
			}
			this.InitializeAsync(file);
#if WINDOWS_PHONE_APP
			if (this._args.HasValue && this._args.Value.Operation == MainPageOperation.Save)
			{
				var saveFile = await StorageFile.GetFileFromPathAsync(this._args.Value.Path);
				this.OnSave(saveFile);
			}
#endif
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) => this.SvgImageControl.SafeUnload();

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

#if WINDOWS_PHONE_APP
			picker.ContinuationData["operation"] = MainPageOperation.Open.ToString();
			picker.PickSingleFileAndContinue();
#else
			var file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				this.PathTextBox.Text = file.Path;
				this.InitializeAsync(file);
			}
#endif
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

#if WINDOWS_PHONE_APP
			picker.ContinuationData["operation"] = MainPageOperation.Save.ToString();
			picker.PickSaveFileAndContinue();
#else
			var file = await picker.PickSaveFileAsync();
			if (file != null)
			{
				this.OnSave(file);
			}
#endif
		}

		public async void OnSave(StorageFile file)
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
				Scaling = 2.0F,
			});
		}
	}
}