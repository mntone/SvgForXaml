using System;
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
	}
}