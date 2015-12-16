using Microsoft.Graphics.Canvas;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.Foundation;
using Windows.UI;
using System.IO;
using Windows.Storage;

namespace Mntone.SvgForXaml
{
	public enum SvgImageRendererFileFormat : byte
	{
		Bitmap = 1,
		Png,
		Jpeg,
		Tiff,
		Gif,
		JpegXr,
	}
	public sealed class SvgImageRendererSettings
	{
		public SvgDocument Document { get; set; }
		public float Scaling { get; set; } = 1.00F;
		public SvgImageRendererFileFormat Format { get; set; } = SvgImageRendererFileFormat.Png;
		public float Quality { get; set; } = 0.9F;
	}

	public static class SvgImageRenderer
	{
		public static Task RendererImageAsync(StorageFile file, SvgDocument svg, SvgImageRendererFileFormat format)
			=> RendererImageAsync(file, new SvgImageRendererSettings { Document = svg, Format = format }, CancellationToken.None);
		public static Task RendererImageAsync(StorageFile file, SvgDocument svg, SvgImageRendererFileFormat format, CancellationToken token)
			=> RendererImageAsync(file, new SvgImageRendererSettings { Document = svg, Format = format }, CancellationToken.None);

		public static Task RendererImageAsync(Stream stream, SvgDocument svg, SvgImageRendererFileFormat format)
			=> ThreadPool.RunAsync(_ => RendererImage(stream, new SvgImageRendererSettings { Document = svg, Format = format })).AsTask();
		public static Task RendererImageAsync(Stream stream, SvgDocument svg, SvgImageRendererFileFormat format, CancellationToken token)
			=> ThreadPool.RunAsync(_ => RendererImage(stream, new SvgImageRendererSettings { Document = svg, Format = format })).AsTask(token);

		public static Task RendererImageAsync(StorageFile file, SvgImageRendererSettings settings)
			=> RendererImageAsync(file, settings, CancellationToken.None);
		public static Task RendererImageAsync(StorageFile file, SvgImageRendererSettings settings, CancellationToken token)
		{
			return ThreadPool.RunAsync(_ =>
			{
				using (var stream = file.OpenStreamForWriteAsync().GetAwaiter().GetResult())
				{
					RendererImage(stream, settings);
				}
			}).AsTask(token);
		}
		public static Task RendererImageAsync(Stream stream, SvgImageRendererSettings settings)
		{
			return ThreadPool.RunAsync(_ => RendererImage(stream, settings)).AsTask();
		}
		public static Task RendererImageAsync(Stream stream, SvgImageRendererSettings settings, CancellationToken token)
		{
			return ThreadPool.RunAsync(_ => RendererImage(stream, settings)).AsTask(token);
		}

		private static void RendererImage(Stream stream, SvgImageRendererSettings settings)
		{
			var svg = settings.Document;
			var viewPort = svg.RootElement.ViewPort;
			if (!viewPort.HasValue) throw new ArgumentException(nameof(settings));

			var width = viewPort.Value.Width;
			var height = viewPort.Value.Height;
			using (var device = CanvasDevice.GetSharedDevice())
			using (var offScreen = new CanvasRenderTarget(device, width, height, settings.Scaling * 96.0F))
			{
				using (var renderer = new Win2dRenderer(offScreen, svg))
				using (var session = offScreen.CreateDrawingSession())
				{
					session.Clear(Colors.Transparent);
					renderer.Render(width, height, session);
				}
				offScreen.SaveAsync(stream.AsRandomAccessStream(), (CanvasBitmapFileFormat)settings.Format, settings.Quality).AsTask().GetAwaiter().GetResult();
			}
		}
	}
}