using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SvgImageControlSample
{
	public sealed partial class App : Application
	{
		public App()
		{
			Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
				Microsoft.ApplicationInsights.WindowsCollectors.Metadata
				| Microsoft.ApplicationInsights.WindowsCollectors.Session
				| Microsoft.ApplicationInsights.WindowsCollectors.UnhandledException);
			this.InitializeComponent();
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached) this.DebugSettings.EnableFrameRateCounter = true;
#endif

			var currentAppView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
			currentAppView.SetPreferredMinSize(new Size(200, 180)); // minimum

			var rootFrame = Window.Current.Content as Frame;
			if (rootFrame == null)
			{
				rootFrame = new Frame();
				Window.Current.Content = rootFrame;
			}
			if (rootFrame.Content == null) rootFrame.Navigate(typeof(MainPage), e.Arguments);
			Window.Current.Activate();
		}
	}
}
