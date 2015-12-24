using System;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#if WINDOWS_APP
using System.Reflection;
#elif WINDOWS_PHONE_APP
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
#endif

namespace Mntone.SvgForXaml.DemoApp
{
	public enum MainPageOperation { Default, Open, Save };
	struct MainPageArgs
	{
		public MainPageOperation Operation { get; set; }
		public string Path { get; set; }
	}

	public sealed partial class App : Application
	{
#if WINDOWS_PHONE_APP
		private TransitionCollection transitions;
#endif

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
			this.Initialize(e.Arguments);
		}

#if WINDOWS_PHONE_APP
		protected override void OnActivated(IActivatedEventArgs args)
		{
			var openArgs = args as FileOpenPickerContinuationEventArgs;
			if (openArgs != null && openArgs.Files.Count != 0)
			{
				this.Initialize(new MainPageArgs
				{
					Operation = (MainPageOperation)Enum.Parse(typeof(MainPageOperation), openArgs.ContinuationData["operation"] as string),
					Path = openArgs.Files[0].Path,
				});
				return;
			}

			var saveArgs = args as FileSavePickerContinuationEventArgs;
			if (saveArgs != null && saveArgs.File != null)
			{
				this.Initialize(string.Empty);
				((MainPage)((Frame)Window.Current.Content).Content).OnSave(saveArgs.File);
				return;
			}

			this.Initialize(string.Empty);
		}
#endif

		private void Initialize(object arguments)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached) this.DebugSettings.EnableFrameRateCounter = true;
#endif

#if WINDOWS_APP && DEBUG
			typeof(Windows.UI.ViewManagement.ApplicationView).GetRuntimeMethod("SetPreferredMinSize", new Type[] { typeof(Windows.Foundation.Size) })
				?.Invoke(Windows.UI.ViewManagement.ApplicationView.GetForCurrentView(), new object[] { new Windows.Foundation.Size(320, 300) });
#elif WINDOWS_UWP
			var currentAppView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
			currentAppView.SetPreferredMinSize(new Size(200, 180)); // minimum
#endif

			var rootFrame = Window.Current.Content as Frame;
			if (rootFrame == null)
			{
				rootFrame = new Frame();
				rootFrame.CacheSize = 1;
				Window.Current.Content = rootFrame;
			}
			if (rootFrame.Content == null)
			{
#if WINDOWS_PHONE_APP
				if (rootFrame.ContentTransitions != null)
				{
					this.transitions = new TransitionCollection();
					foreach (var c in rootFrame.ContentTransitions)
					{
						this.transitions.Add(c);
					}
				}

				rootFrame.ContentTransitions = null;
				rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

				rootFrame.Navigate(typeof(MainPage), arguments);
			}
			Window.Current.Activate();
		}

#if WINDOWS_PHONE_APP
		private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
		{
			var rootFrame = sender as Frame;
			rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
			rootFrame.Navigated -= this.RootFrame_FirstNavigated;
		}
#endif
	}
}
