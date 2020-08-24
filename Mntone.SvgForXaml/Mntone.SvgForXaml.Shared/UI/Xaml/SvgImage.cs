using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Mntone.SvgForXaml.UI.Xaml
{
	[TemplatePart(Name = CANVAS_CONTROL_NAME, Type = typeof(CanvasControl))]
	public sealed class SvgImage : Control
	{
		private const string CANVAS_CONTROL_NAME = "CanvasControl";

		private Win2dRenderer _renderer;
		private CanvasControl _canvasControl;

		public SvgDocument Content
		{
			get { return (SvgDocument)base.GetValue(ContentProperty); }
			set { base.SetValue(ContentProperty, value); }
		}

		public static readonly DependencyProperty ContentProperty
			= DependencyProperty.Register(nameof(Content), typeof(SvgDocument), typeof(SvgImage), new PropertyMetadata(null, OnContentChangedDelegate));

		public SvgImage()
			: base()
		{
			this.DefaultStyleKey = typeof(SvgImage);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this._canvasControl = (CanvasControl)this.GetTemplateChild(CANVAS_CONTROL_NAME);
			this._canvasControl.Draw += OnDraw;
		}

		public async Task LoadFileAsync(StorageFile file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			using (var stream = await WindowsRuntimeStorageExtensions.OpenStreamForReadAsync(file))
			using (var reader = new StreamReader(stream))
			{
				var xml = new XmlDocument();
				xml.LoadXml(reader.ReadToEnd(), new XmlLoadSettings { ProhibitDtd = false });
				var svgDocument = SvgDocument.Parse(xml);

				this.Content = svgDocument;
			}
		}

		public void LoadText(string text)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			var svg = SvgDocument.Parse(text);
			this.Content = svg;
		}

		public void LoadSvg(SvgDocument svg)
		{
			if (svg == null) throw new ArgumentNullException(nameof(svg));

			this.Content = svg;
		}

		private static void OnContentChangedDelegate(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SvgImage)d).OnContentChanged((SvgDocument)e.NewValue);
		private void OnContentChanged(SvgDocument svg)
		{
			if (this._renderer != null)
			{
				this._renderer.Dispose();
				this._renderer = null;
			}

			if (svg != null && this._canvasControl != null)
			{
				this._renderer = new Win2dRenderer(this._canvasControl, svg);
				this._canvasControl?.Invalidate();
			}
		}

		public void SafeUnload()
		{
			if (this._renderer != null)
			{
				this._renderer.Dispose();
				this._renderer = null;
			}
			this._canvasControl.Draw -= OnDraw;
			this._canvasControl.RemoveFromVisualTree();
			this._canvasControl = null;
		}

		private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
		{
			if (this._renderer != null)
			{
				var viewPort = this.Content.RootElement.ViewPort;
				if (viewPort != null)
				{
					var offsetViewPort = Matrix3x2.CreateTranslation(-viewPort.Value.X, -viewPort.Value.Y);
					var iar = viewPort.Value.Width / viewPort.Value.Height;
					var car = sender.ActualWidth / sender.ActualHeight;

					float scale, offset;
					if (iar > car)
					{
						scale = (float)(sender.ActualWidth / viewPort.Value.Width);
						offset = (float)(sender.ActualHeight - scale * viewPort.Value.Height) / 2.0F;
						args.DrawingSession.Transform = offsetViewPort * Matrix3x2.CreateScale(scale) * Matrix3x2.CreateTranslation(0.0F, offset);
					}
					else
					{
						scale = (float)(sender.ActualHeight / viewPort.Value.Height);
						offset = (float)(sender.ActualWidth - scale * viewPort.Value.Width) / 2.0F;
						args.DrawingSession.Transform = offsetViewPort * Matrix3x2.CreateScale(scale) * Matrix3x2.CreateTranslation(offset, 0.0F);
					}
				}

				this._renderer.Render((float)sender.ActualWidth, (float)sender.ActualHeight, args.DrawingSession);
			}
		}
	}
}